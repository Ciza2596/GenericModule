
namespace SaveLoadModule
{
    public interface IIo
    {
        string GetFullPath(string directoryPath, string fileOrDirectoryName);
        void DeleteFile(string fullPath);
    }
}
