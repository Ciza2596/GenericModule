using System.IO;

namespace CizaSaveLoadModule.Implement
{
    public class Io: IIo
    {
        // Takes a directory path and a file or directory name and combines them into a single path.
        public string GetFullPath(string directoryPath, string fileOrDirectoryName)
            => Path.Combine(directoryPath, fileOrDirectoryName);

        public void DeleteFile(string fullPath)
        {
            var fileInfo = new FileInfo(fullPath);
            fileInfo.Delete();
        }
    }
}