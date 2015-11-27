using DuplicateFinder.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace DuplicateFinder.Common.FileHashers
{
    public class FileHasherMd5 : IFileHasher
    {
        public string GetFileHash(string filePath)
        {
            var md5Hasher = MD5.Create();
            byte[] hash;

            using (var stream = new BufferedStream(File.OpenRead(filePath), 1200000))
            {
                hash = md5Hasher.ComputeHash(stream);
            }

            return hash.ToHex();
        }
    }
}
