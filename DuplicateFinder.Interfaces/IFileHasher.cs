namespace DuplicateFinder.Interfaces
{
    public interface IFileHasher
    {
        string GetFileHash(string filePath);
    }
}
