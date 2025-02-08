

using System.IO;

namespace CizaSaveLoadModule
{
    public interface IStreamProvider
    {
        public Stream CreateStream(FileModes fileMode, string fullPath, int bufferSize);
        
    }
}
