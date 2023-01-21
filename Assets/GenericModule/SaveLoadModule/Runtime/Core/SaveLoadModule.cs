using SaveLoadModule.Implement;

namespace SaveLoadModule
{
    public class SaveLoadModule
    {
        //private variable
        private readonly ISaveLoadModuleConfig _saveLoadModuleConfig;
        
        private readonly IWriterProvider _writerProvider;
        private readonly IReaderProvider _readerProvider;


        //public method
        public SaveLoadModule(ISaveLoadModuleConfig saveLoadModuleConfig, IWriterProvider writerProvider, IReaderProvider readerProvider)
        {
            _saveLoadModuleConfig = saveLoadModuleConfig;
            
            _writerProvider = writerProvider;
            _readerProvider = readerProvider;
        }


        public void Save<T>(string key, T data, string path)
        {
            var fullPath = GetFullPath(path);
            var bufferSize = _saveLoadModuleConfig.BufferSize;
            var writer = _writerProvider.CreateWriter(fullPath, bufferSize);

            writer.Write<T>(key, data);
            writer.Save();
        }

        public T Load<T>(string key, string path)
        {
            var fullPath = GetFullPath(path);
            var bufferSize = _saveLoadModuleConfig.BufferSize;
            var reader = _readerProvider.CreateReader(fullPath, bufferSize);
            
            return reader.Read<T>(key);
        }
        
        //private method
        private string GetFullPath(string path)
        {
            var applicationDataPath = _saveLoadModuleConfig.ApplicationDataPath;
            var fullPath = IO.CombinePathAndFilename(applicationDataPath, path);
            return fullPath;
        }

    }
}