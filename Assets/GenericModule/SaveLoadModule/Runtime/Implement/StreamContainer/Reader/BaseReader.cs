using System.IO;

namespace SaveLoadModule.Implement
{
    public abstract class BaseReader : IReader
    {
        public BaseReader(Stream stream)
        {
        }

        public T Read<T>(string key) => throw new System.NotImplementedException();
    }
}