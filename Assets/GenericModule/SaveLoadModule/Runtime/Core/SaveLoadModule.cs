namespace SaveLoadModule
{
    public class SaveLoadModule
    {
        //private variable
        private readonly IWriter _writer;
        private readonly IReader _reader;


        //public method
        public SaveLoadModule(IWriter writer, IReader reader)
        {
            _writer = writer;
            _reader = reader;
        }


        public void Save<T>(string key, T data, string path)
        {
            _writer.Write<T>(key, data);
            _writer.Save();
        }

        public T Load<T>(string key, string path) =>
            _reader.Read<T>(key);
    }
}