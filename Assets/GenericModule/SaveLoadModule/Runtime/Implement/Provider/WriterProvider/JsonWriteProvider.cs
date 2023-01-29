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


        public IWriter CreateWriter(string fullPath, int bufferSize,  bool isWriteHeaderAndFooter,
            System.Text.Encoding encoding)
        {
            var stream = _streamProvider.CreateStream(FileModes.Write, fullPath, bufferSize);
            Assert.IsNotNull(stream, "[JsonWriteProvider::CreateWriter] Create stream is fail.");

            var writer = CreateWrite(stream, isWriteHeaderAndFooter, encoding);
            return writer;
        }


        //private method
        private IWriter CreateWrite(Stream stream,  bool isWriteHeaderAndFooter,
            System.Text.Encoding encoding) =>
            new JsonWriter(stream, isWriteHeaderAndFooter, encoding, _dataTypeController, _reflectionHelper);
    }
}