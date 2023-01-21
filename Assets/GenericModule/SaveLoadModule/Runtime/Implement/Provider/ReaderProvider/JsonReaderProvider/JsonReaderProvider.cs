using System.IO;
using UnityEngine.Assertions;

namespace SaveLoadModule.Implement
{
    public class JsonReaderProvider: IReaderProvider
    {
        //private variable
        private readonly IStreamProvider _streamProvider;


        //public method
        public JsonReaderProvider(IStreamProvider streamProvider) => _streamProvider = streamProvider;

        public IReader CreateReader(string fullPath, int bufferSize)
        {
            var stream = _streamProvider.CreateStream(FileModes.Read, fullPath, bufferSize);
            Assert.IsTrue(stream != null, "");

            var reader = CreateReader(stream);
            return reader;
        }
        
        
        //private method
        private IReader CreateReader(Stream stream) =>
            new JsonReader(stream);
    }
}