using System.IO;

namespace SaveLoadModule.Implement
{
    public class JsonWriter: BaseWriter
    {
        public JsonWriter(Stream stream) : base(stream)
        {
        }
    }
}