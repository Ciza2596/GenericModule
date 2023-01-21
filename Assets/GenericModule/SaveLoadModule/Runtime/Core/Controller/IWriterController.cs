
namespace SaveLoadModule
{
    public interface IWriterController
    {
        void Write<T>(string key, T data);
        void Save();
    }
}
