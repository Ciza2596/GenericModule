
namespace SaveLoadModule
{
    public class SaveLoadModule
    {
        //private variable
        private readonly ISaveLoadModuleConfig _saveLoadModuleConfig;
        private readonly IIo _io;
        
        private readonly IWriterProvider _writerProvider;
        private readonly IReaderProvider _readerProvider;
        

        //public method
        public SaveLoadModule(ISaveLoadModuleConfig saveLoadModuleConfig,IIo io, IWriterProvider writerProvider, IReaderProvider readerProvider)
        {
            _saveLoadModuleConfig = saveLoadModuleConfig;
            _io = io;
            
            _writerProvider = writerProvider;
            _readerProvider = readerProvider;
        }


        public void Save<T>(string key, T data, string path)
        {
            var referenceMode = _saveLoadModuleConfig.ReferenceMode;
            var fullPath = GetFullPath(path);
            var bufferSize = _saveLoadModuleConfig.BufferSize;
            var encoding = _saveLoadModuleConfig.Encoding;
            var writer = _writerProvider.CreateWriter(referenceMode, fullPath, bufferSize, true, encoding);

            writer.Write<T>(key, data);
            
            var reader = _readerProvider.CreateReader(fullPath, bufferSize);
            writer.Save(reader);
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
            var fullPath = _io.CombinePath(applicationDataPath, path);
            return fullPath;
        }

    }
}