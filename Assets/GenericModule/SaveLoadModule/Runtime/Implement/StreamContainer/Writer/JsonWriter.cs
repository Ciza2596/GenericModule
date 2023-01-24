using System.Globalization;
using System.IO;
using DataType;

namespace SaveLoadModule.Implement
{
    public class JsonWriter : BaseWriter
    {
        private const string TRUE_NAME = "true";
        private const string FALSE_NAME = "false";

        private const string DOUBLE_AND_FLOAT_FORMAT_NAME = "R";

        private readonly StreamWriter _streamWriter;

        public JsonWriter(ReferenceModes referenceMode, Stream stream, IDataTypeController dataTypeController,
            IReflectionHelper reflectionHelper) : base(referenceMode, stream, dataTypeController, reflectionHelper)
        {
            _streamWriter = new StreamWriter(stream);
        }

        public override void WritePrimitive(string value)
        {
            _streamWriter.Write("\"");

            // Escape any quotation marks within the string.
            foreach (var c in value)
            {
                switch (c)
                {
                    case '\"':
                    case '“':
                    case '”':
                    case '\\':
                    case '/':
                        _streamWriter.Write('\\');
                        _streamWriter.Write(c);
                        break;
                    case '\b':
                        _streamWriter.Write("\\b");
                        break;
                    case '\f':
                        _streamWriter.Write("\\f");
                        break;
                    case '\n':
                        _streamWriter.Write("\\n");
                        break;
                    case '\r':
                        _streamWriter.Write("\\r");
                        break;
                    case '\t':
                        _streamWriter.Write("\\t");
                        break;
                    default:
                        _streamWriter.Write(c);
                        break;
                }
            }

            _streamWriter.Write("\"");
        }

        public override void WritePrimitive(int value) =>
            _streamWriter.Write(value);

        public override void WritePrimitive(bool value) =>
            _streamWriter.Write(value ? TRUE_NAME : FALSE_NAME);

        public override void WritePrimitive(byte value) =>
            _streamWriter.Write(System.Convert.ToInt32(value));

        public override void WritePrimitive(char value) =>
            WritePrimitive(value.ToString());
        
        public override void WritePrimitive(decimal value) =>
            _streamWriter.Write(value.ToString(CultureInfo.InvariantCulture));

        public override void WritePrimitive(double value) =>
            _streamWriter.Write(value.ToString(DOUBLE_AND_FLOAT_FORMAT_NAME, CultureInfo.InvariantCulture));

        public override void WritePrimitive(float value) =>
            _streamWriter.Write(value.ToString(DOUBLE_AND_FLOAT_FORMAT_NAME, CultureInfo.InvariantCulture));

        public override void WritePrimitive(long value) => 
            _streamWriter.Write(value);

        public override void WritePrimitive(sbyte value) => 
            _streamWriter.Write(System.Convert.ToInt32(value));

        public override void WritePrimitive(short value) => 
            _streamWriter.Write(System.Convert.ToInt32(value));

        public override void WritePrimitive(uint value) =>
            _streamWriter.Write(value);

        public override void WritePrimitive(ulong value) =>
            _streamWriter.Write(value);

        public override void WritePrimitive(ushort value) =>
            _streamWriter.Write(System.Convert.ToInt32(value));

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