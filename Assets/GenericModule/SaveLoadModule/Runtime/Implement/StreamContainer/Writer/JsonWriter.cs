using System.IO;

namespace SaveLoadModule.Implement
{
    public class JsonWriter: BaseWriter
    {
        public JsonWriter(ReferenceModes referenceMode, Stream stream, IDataTypeController dataTypeController) : base(referenceMode, stream, dataTypeController)
        {
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