using System.IO;

namespace SaveLoadModule.Implement
{
    public class FileStreamProvider : IStreamProvider
    {
        //public method
        public Stream CreateStream(FileModes fileMode, string fullPath, int bufferSize)
        {
            var ioFileMode = GetIoFileMode(fileMode);
            var ioFileAccess = GetIoFileAccess(fileMode);

            var fileStream = new FileStream(fullPath, ioFileMode, ioFileAccess, FileShare.None, bufferSize, false);
            return fileStream;
        }


        //private method
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