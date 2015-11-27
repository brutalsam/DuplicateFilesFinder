using System.IO;
using System.Security.Cryptography;
using System.Text;
using DuplicateFinder.Interfaces;

namespace DuplicateFinder.Common.FileHashers
{
    public class FileHasherSha1 : IFileHasher
    {
        public string GetFileHash(string filePath)
        {
            var sha1Hasher = SHA1.Create();
            byte[] hash;

            using (var stream = new BufferedStream(File.OpenRead(filePath), 1200000))
            {
                hash = sha1Hasher.ComputeHash(stream);
            }

            return hash.ToHex();
        }
    }
}
