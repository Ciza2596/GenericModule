using System;
using System.Globalization;
using System.IO;
using System.Text;
using DataType;
using UnityEngine.Assertions;


namespace SaveLoadModule.Implement
{
    public class JsonReader : BaseReader
    {
        private const char END_OF_STREAM_CHAR = (char)65535;
        private readonly IFormatProvider INVARIANT_CULTURE = CultureInfo.InvariantCulture;

        private readonly StreamReader _streamReader;
        private readonly IReflectionHelper _reflectionHelper;

        private readonly int _bufferSize;

        public JsonReader(Stream stream, int bufferSize, IDataTypeController dataTypeController,
            IReflectionHelper reflectionHelper) : base(dataTypeController)
        {
            _streamReader = new StreamReader(stream);
            _reflectionHelper = reflectionHelper;
            _bufferSize = bufferSize;
            
            SkipOpeningBraceOfFile();
        }

        public override Type ReadType()
        {
            var dataType = _dataTypeController.GetOrCreateDataType(typeof(string));
            var typeString = Read<string>(dataType);
            return _reflectionHelper.GetType(typeString);
        }

        public override int ReadInt()
        {
            var valueString = GetValueString();
            return int.Parse(valueString, INVARIANT_CULTURE);
        }

        public override bool ReadBool()
        {
            var valueString = GetValueString();
            return bool.Parse(valueString);
        }

        public override byte ReadByte()
        {
            var valueString = GetValueString();
            return byte.Parse(valueString, INVARIANT_CULTURE);
        }

        public override char ReadChar()
        {
            var readString = ReadString();
            return char.Parse(readString);
        }

        public override decimal ReadDecimal()
        {
            var valueString = GetValueString();
            return decimal.Parse(valueString, INVARIANT_CULTURE);
        }

        public override double ReadDouble()
        {
            var valueString = GetValueString();
            return double.Parse(valueString, INVARIANT_CULTURE);
        }

        public override float ReadFloat()
        {
            var valueString = GetValueString();
            return float.Parse(valueString, INVARIANT_CULTURE);
        }

        public override long ReadLong()
        {
            var valueString = GetValueString();
            return long.Parse(valueString, INVARIANT_CULTURE);
        }

        public override sbyte ReadSbyte()
        {
            var valueString = GetValueString();
            return sbyte.Parse(valueString);
        }

        public override short ReadShort()
        {
            var valueString = GetValueString();
            return short.Parse(valueString);
        }

        public override uint ReadUint()
        {
            var valueString = GetValueString();
            return uint.Parse(valueString);
        }

        public override ulong ReadUlong()
        {
            var valueString = GetValueString();
            return ulong.Parse(valueString);
        }

        public override ushort ReadUshort()
        {
            var valueString = GetValueString();
            return ushort.Parse(valueString);
        }

        public override string ReadString()
        {
            if (ReadQuotationMarkOrNullIgnoreWhitespace())
                return null;

            char c = default;

            var stringBuilder = new StringBuilder();

            while (!IsQuotationMark(c = (char)_streamReader.Read()))
            {
                // If escape mark is found, generate correct escaped character.
                if (c == '\\')
                {
                    c = (char)_streamReader.Read();
                    if (IsEndOfStream(c))
                        throw new FormatException("Reached end of stream while trying to read string literal.");

                    switch (c)
                    {
                        case 'b':
                            c = '\b';
                            break;
                        case 'f':
                            c = '\f';
                            break;
                        case 'n':
                            c = '\n';
                            break;
                        case 'r':
                            c = '\r';
                            break;
                        case 't':
                            c = '\t';
                            break;
                    }
                }

                stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        public override string ReadPropertyName()
        {
            var c = PeekCharIgnoreWhiteSpace();

            if (IsTerminator(c))
                return null;

            if (c == ',')
                ReadCharIgnoreWhiteSpace();

            var propertyName = ReadString();
            ReadCharIgnoreWhiteSpace(':');
            return propertyName;
        }

        public override bool StartReadCollection() => ReadNullOrCharIgnoreWhiteSpace('[');

        public override void EndReadCollection()
        {
        }

        public override bool StartReadCollectionItem()
        {
            if (PeekCharIgnoreWhiteSpace() == ']')
            {
                ReadCharIgnoreWhiteSpace();
                return false;
            }

            return true;
        }

        public override bool EndReadCollectionItem()
        {
            var c = ReadCharIgnoreWhiteSpace();
            return c == ']';
        }

        public override bool StartReadDictionary() =>
            StartReadObject();

        public override void EndReadDictionary()
        {
        }


        public override bool StartReadDictionaryKey()
        {
            if (PeekCharIgnoreWhiteSpace() == '}')
            {
                ReadCharIgnoreWhiteSpace();
                return false;
            }

            return true;
        }

        public override void EndReadDictionaryKey() =>
            ReadCharIgnoreWhiteSpace(':');


        public override void StartReadDictionaryValue()
        {
        }

        public override bool EndReadDictionaryValue()
        {
            var c = ReadCharIgnoreWhiteSpace();
            return c == '}';
        }

        public override void Dispose() =>
            _streamReader.Dispose();

        protected override bool StartReadObject()
        {
            base.StartReadObject();
            return ReadNullOrCharIgnoreWhiteSpace('{');
        }

        protected override void EndReadObject()
        {
            ReadCharIgnoreWhiteSpace('}');
            base.EndReadObject();
        }

        protected override byte[] ReadElement(bool skip = false)
        {
            var writer = skip ? null : new StreamWriter(new MemoryStream(_bufferSize));
            var nesting = 0;
            var c = (char)_streamReader.Peek();

            // Determine if we're skipping a primitive type.
            // First check if it's an opening object or array brace.
            if (!IsOpeningBrace(c))
            {
                // If we're skipping a string, use SkipString().
                if (c == '\"')
                {
                    // Skip initial quotation mark as SkipString() requires this.
                    ReadOrSkipChar(writer, skip);
                    ReadString(writer, skip);
                }
                // Else we just need to read until we reach a closing brace.
                else
                    // While we've not peeked a closing brace.
                    while (!IsEndOfValue((char)_streamReader.Peek()))
                        ReadOrSkipChar(writer, skip);

                if (skip)
                    return null;
                writer.Flush();
                return ((MemoryStream)writer.BaseStream).ToArray();
            }

            // Else, we're skipping a type surrounded by braces.
            // Iterate through every character, logging nesting.
            while (true)
            {
                c = ReadOrSkipChar(writer, skip);

                if (c == END_OF_STREAM_CHAR) // Detect premature end of stream, which denotes missing closing brace.
                    throw new FormatException(
                        "Missing closing brace detected, as end of stream was reached before finding it.");

                // Handle quoted strings.
                // According to the RFC, only '\' and '"' must be escaped in strings.
                if (IsQuotationMark(c))
                {
                    ReadString(writer, skip);
                    continue;
                }

                // Handle braces and other characters.
                switch (c)
                {
                    case '{': // Entered another level of nesting.
                    case '[':
                        nesting++;
                        break;
                    case '}': // Exited a level of nesting.
                    case ']':
                        nesting--;
                        // If nesting < 1, we've come to the end of the object.
                        if (nesting < 1)
                        {
                            if (skip)
                                return null;
                            writer.Flush();
                            return ((MemoryStream)writer.BaseStream).ToArray();
                        }

                        break;
                }
            }
        }

        protected override Type ReadKeyPrefix()
        {
            StartReadObject();

            var propertyName = ReadPropertyName();
            if (propertyName == DataType.DataType.TYPE_TAG)
            {
                var typeString = ReadString();
                var type = _reflectionHelper.GetType(typeString);
                ReadPropertyName();
                return type;
            }

            return null;
        }

        protected override void ReadKeySuffix() =>
            EndReadObject();


        //private method
        private void SkipOpeningBraceOfFile()
        {
            var firstChar = ReadCharIgnoreWhiteSpace();
            Assert.IsTrue(firstChar is '{',$"[JsonReader::SkipOpeningBraceOfFile] File is not valid JSON. Expected '{{' at beginning of file, but found '{firstChar}'.");
        }


        private char ReadCharIgnoreWhiteSpace(bool isIgnoreTrailingWhitespace = true)
        {
            char c;
            // Skip leading whitespace and read char.
            while (IsWhiteSpace(c = (char)_streamReader.Read()))
            {
            }

            // Skip trailing whitespace.
            if (isIgnoreTrailingWhitespace)
                while (IsWhiteSpace((char)_streamReader.Peek()))
                    _streamReader.Read();

            return c;
        }


        private bool ReadNullOrCharIgnoreWhiteSpace(char expectedChar)
        {
            var c = ReadCharIgnoreWhiteSpace();

            // Check for null
            if (c == 'n')
            {
                var chars = new char[3];
                _streamReader.ReadBlock(chars, 0, 3);
                if (chars[0] == 'u' && chars[1] == 'l' && chars[2] == 'l')
                    return true;
            }

            if (c != expectedChar)
            {
                if (c == END_OF_STREAM_CHAR)
                    throw new FormatException($"End of stream reached when expecting expectedChar: {expectedChar}.");

                throw new FormatException($"ExpectedChar {expectedChar}, but found {c}.");
            }

            return false;
        }


        private char ReadCharIgnoreWhiteSpace(char expectedChar)
        {
            char c = ReadCharIgnoreWhiteSpace();
            if (c != expectedChar)
            {
                if (c == END_OF_STREAM_CHAR)
                    throw new FormatException($"End of stream reached when expecting expectedChar: {expectedChar}.");

                throw new FormatException($"ExpectedChar {expectedChar}, but found {c}.");
            }

            return c;
        }


        private bool ReadQuotationMarkOrNullIgnoreWhitespace()
        {
            var c = ReadCharIgnoreWhiteSpace(false); // Don't read trailing whitespace as this is the value.

            if (c == 'n')
            {
                var chars = new char[3];
                _streamReader.ReadBlock(chars, 0, 3);
                if (chars[0] == 'u' && chars[1] == 'l' && chars[2] == 'l')
                    return true;
            }
            else if (!IsQuotationMark(c))
            {
                if (c == END_OF_STREAM_CHAR)
                    throw new FormatException("End of stream reached when expecting quotation mark.");

                throw new FormatException($"Expected quotation mark, found \'{c}\'.");
            }

            return false;
        }


        private char PeekCharIgnoreWhiteSpace()
        {
            char c;
            // Skip leading whitespace and read char.
            while (IsWhiteSpace(c = (char)_streamReader.Peek()))
                _streamReader.Read();
            return c;
        }


        private bool IsWhiteSpace(char c) => (c == ' ' || c == '\t' || c == '\n' || c == '\r');

        private bool IsOpeningBrace(char c) =>
            (c == '{' || c == '[');


        private bool IsEndOfValue(char c) =>
            (c == '}' || c == ' ' || c == '\t' || c == ']' || c == ',' || c == ':' || c == END_OF_STREAM_CHAR ||
             c == '\n' || c == '\r');

        private bool IsTerminator(char c) =>
            (c == '}' || c == ']');

        private bool IsQuotationMark(char c) => c == '\"' || c == '“' || c == '”';

        private bool IsEndOfStream(char c) => c == END_OF_STREAM_CHAR;

        private string GetValueString()
        {
            var stringBuilder = new StringBuilder();
            while (!IsEndOfValue(PeekCharIgnoreWhiteSpace()))
                stringBuilder.Append((char)_streamReader.Read());

            if (stringBuilder.Length == 0)
                return null;

            return stringBuilder.ToString();
        }

        private char ReadOrSkipChar(StreamWriter writer, bool skip)
        {
            var c = (char)_streamReader.Read();
            if (!skip)
                writer.Write(c);

            return c;
        }

        private void ReadString(StreamWriter writer, bool skip = false)
        {
            bool isEndOfString = false;
            // Read to end of string, or throw error if we reach end of stream.
            while (!isEndOfString)
            {
                char c = ReadOrSkipChar(writer, skip);
                switch (c)
                {
                    case END_OF_STREAM_CHAR:
                        throw new FormatException("String without closing quotation mark detected.");
                    case '\\':
                        ReadOrSkipChar(writer, skip);
                        break;
                    default:
                        if (IsQuotationMark(c))
                            isEndOfString = true;
                        break;
                }
            }
        }
    }
}