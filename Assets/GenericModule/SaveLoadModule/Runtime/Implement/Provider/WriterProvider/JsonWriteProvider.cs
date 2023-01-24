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


        public IWriter CreateWriter(ReferenceModes referenceMode, string fullPath, int bufferSize)
        {
            var stream = _streamProvider.CreateStream(FileModes.Write, fullPath, bufferSize);
            Assert.IsTrue(stream != null, "");

            var writer = CreateWrite(referenceMode, stream);
            return writer;
        }


        //private method
        private IWriter CreateWrite(ReferenceModes referenceMode, Stream stream) =>
            new JsonWriter(referenceMode, stream, _dataTypeController);
    }
}