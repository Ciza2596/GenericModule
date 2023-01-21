using System.IO;
using UnityEngine.Assertions;

namespace SaveLoadModule.Implement
{
    public class FileStreamProvider : IStreamProvider
    {
        //private variable
        private readonly IIO _io;
        private readonly IFileStreamProviderConfig _fileStreamProviderConfig;


        //public method
        public FileStreamProvider(IIO io, IFileStreamProviderConfig fileStreamProviderConfig)
        {
            _io = io;
            _fileStreamProviderConfig = fileStreamProviderConfig;
        }

        public Stream CreateStream(FileModes fileMode)
        {
            var path = _fileStreamProviderConfig.Path;
            Assert.IsFalse(fileMode == FileModes.Read && _io.FileExists(path),
                $"[FileStreamProvider::CreateStream] The file doesnt exist on {path} with FileModes.Read.");

            var exitsPath = GetExistPath(path, fileMode);
            var ioFileMode = GetIoFileMode(fileMode);
            var ioFileAccess = GetIoFileAccess(fileMode);
            var bufferSize = _fileStreamProviderConfig.BufferSize;

            var fileStream = new FileStream(exitsPath, ioFileMode, ioFileAccess, FileShare.None, bufferSize, false);
            return fileStream;
        }


        //private method
        private string GetExistPath(string path, FileModes fileMode)
        {
            var directoryPath = _io.GetDirectoryPath(path);

            if (fileMode != FileModes.Read && directoryPath != _io.PersistentDataPath)
                _io.CreateDirectory(directoryPath);

            if (fileMode == FileModes.Write)
                return path + _io.TemporaryFileSuffix;

            return path;
        }

        private FileMode GetIoFileMode(FileModes fileMode)
        {
            if (fileMode == FileModes.Read)
                return FileMode.Open;

            if (fileMode == FileModes.Write)
                return FileMode.Create;

            return FileMode.Append;
        }

        private FileAccess GetIoFileAccess(FileModes fileMode)
        {
            if (fileMode == FileModes.Read)
                return FileAccess.Read;

            if (fileMode == FileModes.Write)
                return FileAccess.Write;

            return FileAccess.Write;
        }
    }
}