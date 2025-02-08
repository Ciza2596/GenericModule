using System;
using System.Globalization;
using System.IO;
using DataType;
using UnityEngine.Scripting;

namespace CizaSaveLoadModule.Implement
{
    [Preserve]
    public class JsonWriter : BaseWriter
    {
        private const string TRUE_NAME = "true";
        private const string FALSE_NAME = "false";

        private const string DOUBLE_AND_FLOAT_FORMAT_NAME = "R";
        private readonly IFormatProvider INVARIANT_CULTURE = CultureInfo.InvariantCulture;

        private readonly StreamWriter _streamWriter;
        private System.Text.Encoding _encoding;

        private bool _isFirstProperty = true;

        [Preserve]
        public JsonWriter(Stream stream, System.Text.Encoding encoding, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(dataTypeController, reflectionHelper)
        {
            _streamWriter = new StreamWriter(stream);
            _encoding = encoding;

            StartWriteFile();
        }

        public override void WritePrimitive(string value)
        {
            _streamWriter.Write(TagUtils.SLASH_WITH_QUOTATION_TAG);

            // Escape any quotation marks within the string.
            foreach (var c in value)
            {
                switch (c)
                {
                    case TagUtils.SLASH_WITH_QUOTATION_TAG:
                    case TagUtils.QUOTATION_TAG_1:
                    case TagUtils.QUOTATION_TAG_2:
                    case TagUtils.DOUBLE_SLASH_TAG:
                    case TagUtils.REVERSE_SLASH_TAG:
                        _streamWriter.Write(TagUtils.DOUBLE_SLASH_TAG);
                        _streamWriter.Write(c);
                        break;
                    case TagUtils.B_TAG_WITH_SLASH:
                        _streamWriter.Write(TagUtils.B_TAG_WITH_DOUBLE_SLASH);
                        break;
                    case TagUtils.F_TAG_WITH_SLASH:
                        _streamWriter.Write(TagUtils.F_TAG_WITH_DOUBLE_SLASH);
                        break;
                    case TagUtils.N_TAG_WITH_SLASH:
                        _streamWriter.Write(TagUtils.N_TAG_WITH_DOUBLE_SLASH);
                        break;
                    case TagUtils.R_TAG_WITH_SLASH:
                        _streamWriter.Write(TagUtils.R_TAG_WITH_DOUBLE_SLASH);
                        break;
                    case TagUtils.T_TAG_WITH_SLASH:
                        _streamWriter.Write(TagUtils.T_TAG_WITH_DOUBLE_SLASH);
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
            _streamWriter.Write(TagUtils.NULL);

        public override void Dispose() =>
            _streamWriter.Dispose();

        public override void StartWriteCollection()
        {
            base.StartWriteCollection();

            _streamWriter.Write(TagUtils.LEFT_SQUARE_BRACE);
            WriteNewLineAndTabs();
        }

        public override void EndWriteCollection()
        {
            base.EndWriteCollection();

            WriteNewLineAndTabs();
            _streamWriter.Write(TagUtils.RIGHT_SQUARE_BRACE);
        }

        public override void StartWriteCollectionItem(int index)
        {
            if (index != 0)
                _streamWriter.Write(TagUtils.COMMA_TAG);
        }

        public override void EndWriteCollectionItem(int index) { }

        public override void StartWriteDictionaryKey(int index)
        {
            if (index != 0)
                _streamWriter.Write(TagUtils.COMMA_TAG);
        }

        public override void EndWriteDictionaryKey(int index) =>
            _streamWriter.Write(TagUtils.COLON_TAG);

        public override void StartWriteDictionaryValue(int index) { }

        public override void EndWriteDictionaryValue(int index) { }

        protected sealed override void StartWriteFile()
        {
            _streamWriter.Write(TagUtils.LEFT_CURLY_BRACE);

            base.StartWriteFile();
        }

        protected override void EndWriteFile()
        {
            base.EndWriteFile();

            WriteNewLineAndTabs();
            _streamWriter.Write(TagUtils.RIGHT_CURLY_BRACE);
        }

        protected override void StartWriteObject(string name)
        {
            base.StartWriteObject(name);

            _isFirstProperty = true;
            _streamWriter.Write(TagUtils.LEFT_CURLY_BRACE);
        }

        protected override void EndWriteObject(string name)
        {
            base.EndWriteObject(name);

            _isFirstProperty = false;
            WriteNewLineAndTabs();
            _streamWriter.Write(TagUtils.RIGHT_CURLY_BRACE);
        }

        protected override void StartWriteProperty(string name)
        {
            base.StartWriteProperty(name);
            WriteCommaIfRequired();
            Write(name);

            _streamWriter.Write(TagUtils.SPACE_TAG);
            _streamWriter.Write(TagUtils.COLON_TAG);
            _streamWriter.Write(TagUtils.SPACE_TAG);
        }

        protected override void EndWriteProperty(string name) { }

        protected override void StartWriteDictionary() =>
            StartWriteObject(null);

        protected override void EndWriteDictionary() =>
            EndWriteObject(null);

        private void WriteNewLineAndTabs()
        {
            _streamWriter.Write(Environment.NewLine);
            for (var i = 0; i < _serializationDepth; i++)
                _streamWriter.Write(TagUtils.T_TAG_WITH_SLASH);
        }

        private void WriteCommaIfRequired()
        {
            if (!_isFirstProperty)
                _streamWriter.Write(TagUtils.COMMA_TAG);
            else
                _isFirstProperty = false;


            WriteNewLineAndTabs();
        }
    }
}