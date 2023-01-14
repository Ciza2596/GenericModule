

namespace SaveLoadModule
{
    public class SaveLoadModule
    {
        public void Save<T>(string key, object data, string path)
        {
            
        }

        public T Load<T>(string key, string path) where T : new()
        {
            T data = new T();
            return data;
        }
    }
}
