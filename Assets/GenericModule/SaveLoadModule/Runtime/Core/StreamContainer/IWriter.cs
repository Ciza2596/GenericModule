
namespace SaveLoadModule
{
    public interface IWriter
    {
        void Write<T>(string key, object value);
        void Save();
    }
}
