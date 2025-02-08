
namespace CizaSaveLoadModule
{
    public interface IWriter
    {
        void Write<T>(string key, object value);
        void Dispose();
    }
}
