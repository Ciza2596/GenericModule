

using System.IO;

namespace SaveLoadModule
{
    public interface IStreamProvider
    {
        public Stream CreateStream(FileModes fileMode, string fullPath, int bufferSize);
        
    }
}
