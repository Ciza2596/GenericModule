namespace SaveLoadModule
{
    public class SaveLoadModule
    {
        //private variable
        private readonly IWriterProvider _writerProvider;
        private readonly IReaderProvider _readerProvider;


        //public method
        public SaveLoadModule(IWriterProvider writerProvider, IReaderProvider readerProvider)
        {
            _writerProvider = writerProvider;
            _readerProvider = readerProvider;
        }


        public void Save<T>(string key, T data, string path)
        {
            var writer = _writerProvider.CreateWriter(path);

            writer.Write<T>(key, data);
            writer.Save();
        }

        public T Load<T>(string key, string path)
        {
            var reader = _readerProvider.CreateReader(path);
            return reader.Read<T>(key);
        }
    }
}