using System.IO;
using DataType;
using UnityEngine.Assertions;

namespace SaveLoadModule.Implement
{
    public class JsonReaderProvider: IReaderProvider
    {
        //private variable
        private readonly IStreamProvider _streamProvider;
        private readonly IDataTypeController _dataTypeController;
        private readonly IReflectionHelper _reflectionHelper;


        //public method
        public JsonReaderProvider(IStreamProvider streamProvider, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper)
        {
            _streamProvider = streamProvider;
            _dataTypeController = dataTypeController;
            _reflectionHelper = reflectionHelper;
        }

        public IReader CreateReader(string fullPath, int bufferSize)
        {
            var stream = _streamProvider.CreateStream(FileModes.Read, fullPath, bufferSize);
            Assert.IsNotNull(stream, $"[JsonReaderProvider::CreateReader] stream is null by fullPath: {fullPath}.");

            var reader = CreateReader(stream);
            return reader;
        }
        
        
        //private method
        private IReader CreateReader(Stream stream) =>
            new JsonReader(stream, _dataTypeController, _reflectionHelper);
    }
}