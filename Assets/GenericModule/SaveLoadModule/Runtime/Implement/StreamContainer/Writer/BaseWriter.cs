using System.IO;

namespace SaveLoadModule.Implement
{
    public abstract class BaseWriter: IWriter
    {
        public BaseWriter(Stream stream)
        {
        }


        public void Write<T>(string key, T data)
        {
            
        }

        public void Save()
        {
            
        }
    }
}