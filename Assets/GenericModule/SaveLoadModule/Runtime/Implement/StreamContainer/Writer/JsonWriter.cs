using System.IO;

namespace SaveLoadModule.Implement
{
    public class JsonWriter: BaseWriter
    {
        public JsonWriter(Stream stream, IDataTypeController dataTypeController) : base(stream, dataTypeController)
        {
        }
    }
}