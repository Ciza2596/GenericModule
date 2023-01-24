using System.IO;
using DataType;

namespace SaveLoadModule.Implement
{
    public class JsonWriter: BaseWriter
    {
        public JsonWriter(ReferenceModes referenceMode, Stream stream, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(referenceMode, stream, dataTypeController, reflectionHelper)
        {
        }

        public override void WriteNull()
        {
            throw new System.NotImplementedException();
        }

        protected override void EndWriteProperty(string name)
        {
            throw new System.NotImplementedException();
        }

        protected override void StartWriteDictionary()
        {
            throw new System.NotImplementedException();
        }

        protected override void EndWriteDictionary()
        {
            throw new System.NotImplementedException();
        }
    }
}