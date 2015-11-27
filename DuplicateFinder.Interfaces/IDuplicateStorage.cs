using System.Collections.Generic;

namespace DuplicateFinder.Interfaces
{
    public interface IDuplicateStorage
    {
        void AddFile(string hash, string fileName);
        IEnumerable<List<string>> GetDuplicates();
    }
}
