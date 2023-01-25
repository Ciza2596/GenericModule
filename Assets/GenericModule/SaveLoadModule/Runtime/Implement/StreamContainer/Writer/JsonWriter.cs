using System;
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
        private readonly IFormatProvider INVARIANT_CULTURE = CultureInfo.InvariantCulture;


        private readonly StreamWriter _streamWriter;
        private bool _isWriteHeaderAndFooter;
        private System.Text.Encoding _encoding;


        private bool _isFirstProperty = true;

        public JsonWriter(ReferenceModes referenceMode, Stream stream, bool isWriteHeaderAndFooter,
            System.Text.Encoding encoding, IDataTypeController dataTypeController,
            IReflectionHelper reflectionHelper) : base(
            referenceMode, dataTypeController, reflectionHelper)
        {
            _streamWriter = new StreamWriter(stream);
            _isWriteHeaderAndFooter = isWriteHeaderAndFooter;
            _encoding = encoding;
            StartWriteFile();
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
            _streamWriter.Write(Convert.ToInt32(value));

        public override void WritePrimitive(char value) =>
            WritePrimitive(value.ToString());

        public override void WritePrimitive(decimal value) =>
            _streamWriter.Write(value.ToString(INVARIANT_CULTURE));

        public override void WritePrimitive(double value) =>
            _streamWriter.Write(value.ToString(DOUBLE_AND_FLOAT_FORMAT_NAME, INVARIANT_CULTURE));

        public override void WritePrimitive(float value) =>
            _streamWriter.Write(value.ToString(DOUBLE_AND_FLOAT_FORMAT_NAME, INVARIANT_CULTURE));

        public override void WritePrimitive(long value) =>
            _streamWriter.Write(value);

        public override void WritePrimitive(sbyte value) =>
            _streamWriter.Write(Convert.ToInt32(value));

        public override void WritePrimitive(short value) =>
            _streamWriter.Write(Convert.ToInt32(value));

        public override void WritePrimitive(uint value) =>
            _streamWriter.Write(value);

        public override void WritePrimitive(ulong value) =>
            _streamWriter.Write(value);

        public override void WritePrimitive(ushort value) =>
            _streamWriter.Write(Convert.ToInt32(value));

        public override void WriteNull() =>
            _streamWriter.Write("null");

        public override void Dispose() =>
            _streamWriter.Dispose();
        

        public override void StartWriteCollection()
        {
            base.StartWriteCollection();

            _streamWriter.Write('[');
            WriteNewLineAndTabs();
        }

        public override void EndWriteCollection()
        {
            base.EndWriteCollection();

            WriteNewLineAndTabs();
            _streamWriter.Write(']');
        }

        public override void StartWriteCollectionItem(int index)
        {
            if (index != 0)
                _streamWriter.Write(',');
        }

        public override void EndWriteCollectionItem(int index)
        {
        }

        public override void StartWriteDictionaryKey(int index)
        {
            if (index != 0)
                _streamWriter.Write(',');
        }

        public override void EndWriteDictionaryKey(int index) =>
            _streamWriter.Write(':');


        public override void StartWriteDictionaryValue(int index)
        {
        }

        public override void EndWriteDictionaryValue(int index)
        {
        }


        protected override void StartWriteFile()
        {
            if (_isWriteHeaderAndFooter)
                _streamWriter.Write('{');

            base.StartWriteFile();
        }

        protected override void EndWriteFile()
        {
            base.EndWriteFile();

            WriteNewLineAndTabs();
            if (_isWriteHeaderAndFooter)
                _streamWriter.Write('}');
        }

        protected override void StartWriteObject(string name)
        {
            base.StartWriteObject(name);

            _isFirstProperty = true;
            _streamWriter.Write('{');
        }

        protected override void EndWriteObject(string name)
        {
            base.EndWriteObject(name);

            _isFirstProperty = false;
            WriteNewLineAndTabs();
            _streamWriter.Write('}');
        }


        protected override void StartWriteProperty(string name)
        {
            base.StartWriteProperty(name);
            WriteCommaIfRequired();
            Write(name);

            _streamWriter.Write(' ');
            _streamWriter.Write(':');
            _streamWriter.Write(' ');
        }

        protected override void EndWriteProperty(string name)
        {
        }

        protected override void WriteRawProperty(string name, byte[] value)
        {
            StartWriteProperty(name);
            _streamWriter.Write(_encoding.GetString(value, 0, value.Length));
            EndWriteProperty(name);
        }

        protected override void StartWriteDictionary()
        {
            StartWriteObject(null);
        }

        protected override void EndWriteDictionary()
        {
        }


        private void WriteNewLineAndTabs()
        {
            _streamWriter.Write(Environment.NewLine);
            for (var i = 0; i < _serializationDepth; i++)
                _streamWriter.Write('\t');
        }

        private void WriteCommaIfRequired()
        {
            if (!_isFirstProperty)
                _streamWriter.Write(',');
            else
                _isFirstProperty = false;

            WriteNewLineAndTabs();
        }
    }
}