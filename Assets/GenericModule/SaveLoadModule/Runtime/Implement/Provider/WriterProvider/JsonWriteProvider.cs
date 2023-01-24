using System.IO;
using DataType;
using UnityEngine.Assertions;

namespace SaveLoadModule.Implement
{
    public class JsonWriteProvider : IWriterProvider
    {
        //private variable
        private readonly IStreamProvider _streamProvider;
        private readonly IDataTypeController _dataTypeController;
        private readonly IReflectionHelper _reflectionHelper;


        //public method
        public JsonWriteProvider(IStreamProvider streamProvider, IDataTypeController dataTypeController)
        {
            _streamProvider = streamProvider;
            _dataTypeController = dataTypeController;
        }


        public IWriter CreateWriter(ReferenceModes referenceMode, string fullPath, int bufferSize)
        {
            var stream = _streamProvider.CreateStream(FileModes.Write, fullPath, bufferSize);
            Assert.IsNotNull(stream, "[JsonWriteProvider::CreateWriter] Create stream is fail.");

            var writer = CreateWrite(referenceMode, stream);
            return writer;
        }


        //private method
        private IWriter CreateWrite(ReferenceModes referenceMode, Stream stream) =>
            new JsonWriter(referenceMode, stream, _dataTypeController, _reflectionHelper);
    }
}