
namespace SaveLoadModule
{
    public interface IWriter
    {
        void Write<T>(string key, T data);
        void Save();
    }
}
