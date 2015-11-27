using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using DuplicateFinder.Common;
using DuplicateFinder.Common.FileHashers;
using log4net;
using log4net.Config;

namespace DuplicateFinder
{
    class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        private static object syncObject = new object();
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            if (args.Count() != 1)
            {
                LogMessage("Please set path to search duplicates (sample C:\temp)");
                Console.ReadKey();
                Environment.Exit(0);
            }

            var folder = args.First();

            if (!Directory.Exists(folder))
            {
                LogMessage("folder [{0}] does not exists", folder);
                Console.ReadKey();
                Environment.Exit(0);
            }

            Stopwatch watch = new Stopwatch();
            LogMessage("Starting search files in directory [{0}] ...", folder);

            watch.Start();
                
            var fileList = WinApiSearch.RecursiveScan(folder);
                
            watch.Stop();
            LogMessage("{0} files found in directory [{1}], time spent: [{2}]", fileList.Count(), folder,
                watch.Elapsed);

            var duplicateFilesFinder = new DuplicateFilesFinder(new FileHasherMd5(), fileList, Logger);
            duplicateFilesFinder.FileProcessed += OnFileProcessed;
            watch.Restart();
            
            var duplicates = duplicateFilesFinder.GetDuplicates();

            watch.Stop();
            LogMessage("");
            LogMessage("{0} duplicate groups found", duplicates.Count());
            LogMessage("{0} possible duplicates on this groups, time spent: [{1}]", duplicates.Sum(x => x.Count()),
                watch.Elapsed);

            //using (StreamWriter sw = File.AppendText(@"C:\Pub\duplicates.txt"))
            //{
            //    foreach (var files in duplicates)
            //    {
            //        sw.WriteLine("=======================");
            //        foreach (var file in files)
            //        {
            //            sw.WriteLine(file.Path);
            //        }
            //    }
            //}

            Console.ReadKey();
        }

        private static void OnFileProcessed(object sender, int fileschecked)
        {
            lock (syncObject)
            {
                Console.CursorLeft = 0;
                Console.Write("Files checked: {0}", fileschecked);
            }
        }

        private static void LogMessage(string message, params object[] args)
        {
            Logger.Info(String.Format(message, args));
        }
    }
}
