using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DuplicateFinder.Interfaces;
using log4net;

namespace DuplicateFinder.Common
{
    public delegate void FileHashRecieved(object sender, int filesChecked);

    public class DuplicateFilesFinder
    {
        private readonly object syncObject = new object();
        private readonly ILog logger;
        private readonly IFileHasher hasher;
        private IEnumerable<Win32FileInfo> fileList;

        public event FileHashRecieved FileProcessed;

        public DuplicateFilesFinder(IFileHasher hasher, IEnumerable<Win32FileInfo> fileList, ILog log)
        {
            this.hasher = hasher;
            this.fileList = fileList;
            logger = log;
        }

        public IEnumerable<IEnumerable<Win32FileInfo>> GetDuplicates()
        {
            var filesGroupedBySize = fileList.ToLookup(key => key.Size, data => data).Where(x => x.Count() > 1);
            var sizeFilteredList = filesGroupedBySize.SelectMany(x => x);
            var count = sizeFilteredList.Count();
            LogMessage("{0} files after size filtering", count);

            var counter = 0;
            Parallel.ForEach(sizeFilteredList, new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount},
                file =>
                    //foreach (var file in sizeFilteredList)
                {
                    try
                    {
                        file.Hash = hasher.GetFileHash(file.Path);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        WriteWarning(string.Format("UnauthorizedAccessException, Can't get access to the File {0}",
                            file.Path));
                    }
                    catch (IOException)
                    {
                        WriteWarning(String.Format("IOException, Can't get access to the file {0}", file.Path));
                    }

                    counter = Interlocked.Increment(ref counter);
                    OnFileProcessed(counter);
                });

            var result =
                sizeFilteredList.Where(element => element.Hash != string.Empty)
                    .ToLookup(key => key.Hash, element => element)
                    .Select(group => group.AsEnumerable());

            return result;
        }

        protected virtual void OnFileProcessed(int fileschecked)
        {
            var handler = FileProcessed;
            if (handler != null) handler(this, fileschecked);
        }

        private void WriteWarning(string message)
        {
            if (logger != null)
            {
                lock (syncObject)
                {
                    logger.Warn(message);
                }
            }
        }

        private void LogMessage(string message, params object[] args)
        {
            if (logger != null)
            {
                lock (syncObject)
                {
                    logger.Info(string.Format(message, args));
                }
            }
        }
    }
}
