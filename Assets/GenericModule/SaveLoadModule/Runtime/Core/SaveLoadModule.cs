
namespace SaveLoadModule
{
    public class SaveLoadModule
    {
        //private variable
        private readonly ISaveLoadModuleConfig _saveLoadModuleConfig;
        private readonly IIo _io;
        
        private readonly IWriterProvider _writerProvider;
        private readonly IReaderProvider _readerProvider;
        

        //public constructor
        public SaveLoadModule(ISaveLoadModuleConfig saveLoadModuleConfig,IIo io, IWriterProvider writerProvider, IReaderProvider readerProvider)
        {
            _saveLoadModuleConfig = saveLoadModuleConfig;
            _io = io;
            
            _writerProvider = writerProvider;
            _readerProvider = readerProvider;
        }


        //public method
        public void Save<T>(string key, T data, string filePath = null)
        {
            var fullPath = GetFullPath(filePath);
            var bufferSize = _saveLoadModuleConfig.BufferSize;
            var encoding = _saveLoadModuleConfig.Encoding;
            var writer = _writerProvider.CreateWriter(fullPath, bufferSize, true, encoding);

            writer.Write<T>(key, data);
            writer.Dispose();
        }

        public T Load<T>(string key, string filePath = null)
        {
            var fullPath = GetFullPath(filePath);
            var bufferSize = _saveLoadModuleConfig.BufferSize;
            var reader = _readerProvider.CreateReader(fullPath, bufferSize);

            return reader.Read<T>(key);
        }
        
        //private method
        private string GetFullPath(string filePath)
        {
            var path = string.IsNullOrWhiteSpace(filePath) ? _saveLoadModuleConfig.DefaultFilePath : filePath;
            var applicationDataPath = _saveLoadModuleConfig.ApplicationDataPath;
            var fullPath = _io.CombinePath(applicationDataPath, path);
            return fullPath;
        }
    }
}