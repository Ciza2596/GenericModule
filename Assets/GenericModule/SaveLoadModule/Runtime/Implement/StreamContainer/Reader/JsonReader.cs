using System;
using System.IO;

namespace SaveLoadModule.Implement
{
    public class JsonReader : BaseReader
    {
        public JsonReader(Stream stream, IDataTypeController dataTypeController) : base(stream, dataTypeController)
        {
        }

        public override string ReadPropertyName() => throw new NotImplementedException();

        protected override Type ReadType<T>() => throw new NotImplementedException();

        protected override byte[] ReadElement(bool skip = false) => throw new NotImplementedException();
    }
}