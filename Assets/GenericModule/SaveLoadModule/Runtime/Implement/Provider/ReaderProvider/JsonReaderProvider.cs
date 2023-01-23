using System.IO;
using UnityEngine.Assertions;

namespace SaveLoadModule.Implement
{
    public class JsonReaderProvider: IReaderProvider
    {
        //private variable
        private readonly IStreamProvider _streamProvider;
        private readonly IDataTypeController _dataTypeController;


        //public method
        public JsonReaderProvider(IStreamProvider streamProvider, IDataTypeController dataTypeController)
        {
            _streamProvider = streamProvider;
            _dataTypeController = dataTypeController;
        }

        public IReader CreateReader(string fullPath, int bufferSize)
        {
            var stream = _streamProvider.CreateStream(FileModes.Read, fullPath, bufferSize);
            Assert.IsTrue(stream != null, "");

            var reader = CreateReader(stream);
            return reader;
        }
        
        
        //private method
        private IReader CreateReader(Stream stream) =>
            new JsonReader(stream, _dataTypeController);
    }
}