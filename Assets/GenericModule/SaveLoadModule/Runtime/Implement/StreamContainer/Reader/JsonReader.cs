using System.IO;

namespace SaveLoadModule.Implement
{
    public class JsonReader : BaseReader
    {
        public JsonReader(Stream stream) : base(stream)
        {
        }
    }
}