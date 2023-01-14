using System;

namespace SaveLoadModule
{
    public interface IIO
    {

        public string PersistentDataPath { get; }

        public string BackupFileSuffix { get; }
        public string TemporaryFileSuffix { get; }

        
        public DateTime GetTimestamp(string filePath);

        public string GetExtension(string path);

        public void DeleteFile(string filePath);

        public bool FileExists(string filePath);
        public void MoveFile(string sourcePath, string destPath);
        public void CopyFile(string sourcePath, string destPath);

        public void MoveDirectory(string sourcePath, string destPath);
        public void CreateDirectory(string directoryPath);
        public bool DirectoryExists(string directoryPath);
        
        public string GetDirectoryPath(string path, char seperator = '/');

        public bool UsesForwardSlash(string path);

        public string CombinePathAndFilename(string directoryPath, string fileOrDirectoryName);

        public string[] GetDirectories(string path, bool getFullPaths = true);

        public void DeleteDirectory(string directoryPath);

        public string[] GetFiles(string path, bool getFullPaths = true);

        public byte[] ReadAllBytes(string path);

        public void WriteAllBytes(string path, byte[] bytes);
        
    }
}
