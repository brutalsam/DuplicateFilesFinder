using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using DuplicateFinder.Interfaces;

namespace DuplicateFinder.Common.FileHashers
{
    public class FileHasherSha256 : IFileHasher
    {
        public string GetFileHash(string filePath)
        {
            var sha256Hasher = SHA256.Create();
            byte[] hash;

            using (var stream = new BufferedStream(File.OpenRead(filePath), 1200000))
            {
                hash = sha256Hasher.ComputeHash(stream);
            }

            return hash.ToHex();
        }
    }
}
