using System;
using System.Globalization;
using System.IO;
using System.Text;
using DataType;


namespace SaveLoadModule.Implement
{
    public class JsonReader : BaseReader
    {
        private const char END_OF_STREAM_CHAR = (char)65535;
        private readonly IFormatProvider INVARIANT_CULTURE = CultureInfo.InvariantCulture;

        private readonly StreamReader _streamReader;
        private readonly IReflectionHelper _reflectionHelper;

        public JsonReader(Stream stream, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(dataTypeController)
        {
            _streamReader = new StreamReader(stream);
            _reflectionHelper = reflectionHelper;

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
            var valueString = GetValueString();
            return char.Parse(valueString);
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
                return string.Empty;

            if (c == ',')
                ReadCharIgnoreWhiteSpace();

            var propertyName = ReadString();
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
        
        public override void EndReadDictionary() { }
        

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
        

        public override void StartReadDictionaryValue() { }

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
        

        protected override Type ReadType<T>() => throw new NotImplementedException();

        protected override byte[] ReadElement(bool skip = false) => throw new NotImplementedException();

        protected override Type ReadKeyPrefix()
        {
            StartReadObject();


            var propertyName = ReadPropertyName();
            if (propertyName == DataType.DataType.TYPE_FIELD_NAME)
            {
                var typeString = ReadString();
                var type = _reflectionHelper.GetType(typeString);
                return type;
            }

            return null;
        }

        protected override void ReadKeySuffix() =>
            EndReadObject();


        //private method
        private char ReadCharIgnoreWhiteSpace(bool isIgnoreTrailingWhitespace = true)
        {
            char c;
            // Skip leading whitespace and read char.
            while (IsWhiteSpace(c = (char)_streamReader.Read())) { }

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
                if(c == END_OF_STREAM_CHAR)
                    throw new FormatException($"End of stream reached when expecting expectedChar: {expectedChar}.");
                
                throw new FormatException($"ExpectedChar {expectedChar}, but found {c}.");
            }

            return false;
        }
        
        
        private char ReadCharIgnoreWhiteSpace(char expectedChar)
        {
            char c = ReadCharIgnoreWhiteSpace();
            if(c != expectedChar)
            {
                if(c == END_OF_STREAM_CHAR)
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
                if ((char)chars[0] == 'u' && (char)chars[1] == 'l' && (char)chars[2] == 'l')
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
    }
}