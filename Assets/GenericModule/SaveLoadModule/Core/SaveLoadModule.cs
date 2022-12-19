

namespace SaveLoadModule
{
    public class SaveLoadModule
    {

        public void Save<T>(object data, string path)
        {
            
        }

        public T Load<T>(string path) where T : new()
        {
            T data = new T();
            return data;
        }
    }
}
