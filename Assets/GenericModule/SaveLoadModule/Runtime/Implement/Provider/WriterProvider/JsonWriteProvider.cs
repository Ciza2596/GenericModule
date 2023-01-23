using System.IO;
using UnityEngine.Assertions;

namespace SaveLoadModule.Implement
{
    public class JsonWriteProvider : IWriterProvider
    {
        //private variable
        private readonly IStreamProvider _streamProvider;
        private readonly IDataTypeController _dataTypeController;


        //public method
        public JsonWriteProvider(IStreamProvider streamProvider, IDataTypeController dataTypeController)
        {
            _streamProvider = streamProvider;
            _dataTypeController = dataTypeController;
        }


        public IWriter CreateWriter(string fullPath, int bufferSize)
        {
            var stream = _streamProvider.CreateStream(FileModes.Write, fullPath, bufferSize);
            Assert.IsTrue(stream != null, "");

            var writer = CreateWrite(stream);
            return writer;
        }


        //private method
        private IWriter CreateWrite(Stream stream) =>
            new JsonWriter(stream, _dataTypeController);
    }
}