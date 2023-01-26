using System.IO;

namespace SaveLoadModule.Implement
{
    public class Io: IIo
    {
        // Takes a directory path and a file or directory name and combines them into a single path.
        public string CombinePath(string directoryPath, string fileOrDirectoryName)
            => Path.Combine(directoryPath, fileOrDirectoryName);
    }
}