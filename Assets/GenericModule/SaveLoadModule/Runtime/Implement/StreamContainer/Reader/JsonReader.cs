using System;
using System.Globalization;
using System.IO;
using System.Text;
using DataType;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace CizaSaveLoadModule.Implement
{
	public class JsonReader : BaseReader
	{
		private readonly IFormatProvider INVARIANT_CULTURE = CultureInfo.InvariantCulture;

		private readonly StreamReader      _streamReader;
		private readonly IReflectionHelper _reflectionHelper;

		private readonly int _bufferSize;

		[Preserve]
		public JsonReader(Stream stream, int bufferSize, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(dataTypeController)
		{
			_streamReader     = new StreamReader(stream);
			_reflectionHelper = reflectionHelper;
			_bufferSize       = bufferSize;

			SkipOpeningBraceOfFile();
		}

		public override Type ReadType()
		{
			var dataType   = _dataTypeController.GetOrCreateDataType(typeof(string));
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
				if (c == TagUtils.DOUBLE_SLASH_TAG)
				{
					c = (char)_streamReader.Read();
					if (IsEndOfStream(c))
						throw new FormatException("[JsonReader::ReadString] Reached end of stream while trying to read string literal.");

					switch (c)
					{
						case TagUtils.B_TAG:
							c = TagUtils.B_TAG_WITH_SLASH;
							break;
						case TagUtils.F_TAG:
							c = TagUtils.F_TAG_WITH_SLASH;
							break;
						case TagUtils.N_TAG:
							c = TagUtils.N_TAG_WITH_SLASH;
							break;
						case TagUtils.R_TAG:
							c = TagUtils.R_TAG_WITH_SLASH;
							break;
						case TagUtils.T_TAG:
							c = TagUtils.T_TAG_WITH_SLASH;
							break;
					}
				}

				stringBuilder.Append(c);
			}

			return stringBuilder.ToString();
		}

		[Preserve]
		public override string ReadPropertyName()
		{
			var c = PeekCharIgnoreWhiteSpace();

			if (IsTerminator(c))
				return null;

			if (c == TagUtils.COMMA_TAG)
				ReadCharIgnoreWhiteSpace();

			var propertyName = ReadString();
			ReadCharIgnoreWhiteSpace(TagUtils.COLON_TAG);
			return propertyName;
		}

		public override bool StartReadCollection() => ReadNullOrCharIgnoreWhiteSpace(TagUtils.LEFT_SQUARE_BRACE);

		public override void EndReadCollection() { }

		public override bool StartReadCollectionItem()
		{
			if (PeekCharIgnoreWhiteSpace() == TagUtils.RIGHT_SQUARE_BRACE)
			{
				ReadCharIgnoreWhiteSpace();
				return false;
			}

			return true;
		}

		public override bool EndReadCollectionItem()
		{
			var c = ReadCharIgnoreWhiteSpace();
			return c == TagUtils.RIGHT_SQUARE_BRACE;
		}

		public override bool StartReadDictionary() =>
			StartReadObject();

		public override void EndReadDictionary() { }

		public override bool StartReadDictionaryKey()
		{
			if (PeekCharIgnoreWhiteSpace() == TagUtils.RIGHT_CURLY_BRACE)
			{
				ReadCharIgnoreWhiteSpace();
				return false;
			}

			return true;
		}

		public override void EndReadDictionaryKey() =>
			ReadCharIgnoreWhiteSpace(TagUtils.COLON_TAG);

		public override void StartReadDictionaryValue() { }

		public override bool EndReadDictionaryValue()
		{
			var c = ReadCharIgnoreWhiteSpace();
			return c == TagUtils.RIGHT_CURLY_BRACE;
		}

		public override void Dispose() =>
			_streamReader.Dispose();

		protected override bool StartReadObject()
		{
			base.StartReadObject();
			return ReadNullOrCharIgnoreWhiteSpace(TagUtils.LEFT_CURLY_BRACE);
		}

		protected override void EndReadObject()
		{
			ReadCharIgnoreWhiteSpace(TagUtils.RIGHT_CURLY_BRACE);
			base.EndReadObject();
		}

		[Preserve]
		protected override byte[] ReadElement(bool skip = false)
		{
			var writer  = skip ? null : new StreamWriter(new MemoryStream(_bufferSize));
			var nesting = 0;
			var c       = (char)_streamReader.Peek();

			// Determine if we're skipping a primitive type.
			// First check if it's an opening object or array brace.
			if (!IsOpeningBrace(c))
			{
				// If we're skipping a string, use SkipString().
				if (c == TagUtils.SLASH_WITH_QUOTATION_TAG)
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

				if (c == TagUtils.END_OF_STREAM_TAG) // Detect premature end of stream, which denotes missing closing brace.
					throw new FormatException("Missing closing brace detected, as end of stream was reached before finding it.");

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
					case TagUtils.LEFT_CURLY_BRACE: // Entered another level of nesting.
					case TagUtils.LEFT_SQUARE_BRACE:
						nesting++;
						break;
					case TagUtils.RIGHT_CURLY_BRACE: // Exited a level of nesting.
					case TagUtils.RIGHT_SQUARE_BRACE:
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
			if (propertyName == TagUtils.TYPE_TAG)
			{
				var typeString = ReadString();
				var type       = _reflectionHelper.GetType(typeString);
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
			Assert.IsTrue(firstChar is TagUtils.LEFT_CURLY_BRACE, $"[JsonReader::SkipOpeningBraceOfFile] File is not valid JSON. Expected '{{' at beginning of file, but found '{firstChar}'.");
		}

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
			if (c == TagUtils.N_TAG)
			{
				var chars = new char[3];
				_streamReader.ReadBlock(chars, 0, 3);
				if (chars[0] == TagUtils.U_TAG && chars[1] == TagUtils.L_TAG && chars[2] == TagUtils.L_TAG)
					return true;
			}

			// # need remove !IsQuotationMark(c)
			if (c != expectedChar)
			{
				if (c == TagUtils.END_OF_STREAM_TAG)
					throw new FormatException($"[JsonReader::ReadNullOrCharIgnoreWhiteSpace] End of stream reached when expecting expectedChar: {expectedChar}.");

				throw new FormatException($"[JsonReader::ReadNullOrCharIgnoreWhiteSpace] ExpectedChar {expectedChar}, but found {c}.");
			}

			return false;
		}

		private char ReadCharIgnoreWhiteSpace(char expectedChar)
		{
			char c = ReadCharIgnoreWhiteSpace();
			if (c != expectedChar)
			{
				if (c == TagUtils.END_OF_STREAM_TAG)
					throw new FormatException($"[JsonReader::ReadCharIgnoreWhiteSpace] End of stream reached when expecting expectedChar: {expectedChar}.");

				throw new FormatException($"[JsonReader::ReadCharIgnoreWhiteSpace] ExpectedChar {expectedChar}, but found {c}.");
			}

			return c;
		}

		private bool ReadQuotationMarkOrNullIgnoreWhitespace()
		{
			var c = ReadCharIgnoreWhiteSpace(false); // Don't read trailing whitespace as this is the value.

			if (c == TagUtils.N_TAG)
			{
				var chars = new char[3];
				_streamReader.ReadBlock(chars, 0, 3);
				if (chars[0] == TagUtils.U_TAG && chars[1] == TagUtils.L_TAG && chars[2] == TagUtils.L_TAG)
					return true;
			}
			else if (!IsQuotationMark(c))
			{
				if (c == TagUtils.END_OF_STREAM_TAG)
					throw new FormatException("[JsonReader::ReadQuotationMarkOrNullIgnoreWhitespace] End of stream reached when expecting quotation mark.");

				throw new FormatException($"[JsonReader::ReadQuotationMarkOrNullIgnoreWhitespace] Expected quotation mark, found \'{c}\'.");
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

		private bool IsWhiteSpace(char c) =>
			(c == TagUtils.SPACE_TAG || c == TagUtils.T_TAG_WITH_SLASH || c == TagUtils.N_TAG_WITH_SLASH || c == TagUtils.R_TAG_WITH_SLASH);

		private bool IsOpeningBrace(char c) =>
			(c == TagUtils.LEFT_CURLY_BRACE || c == TagUtils.LEFT_SQUARE_BRACE);

		private bool IsEndOfValue(char c) =>
			(c == TagUtils.RIGHT_CURLY_BRACE || c == TagUtils.SPACE_TAG || c == TagUtils.T_TAG_WITH_SLASH || c == TagUtils.RIGHT_SQUARE_BRACE || c == TagUtils.COMMA_TAG || c == TagUtils.COLON_TAG || c == TagUtils.END_OF_STREAM_TAG || c == TagUtils.N_TAG_WITH_SLASH || c == TagUtils.R_TAG_WITH_SLASH);

		private bool IsTerminator(char c) =>
			(c == TagUtils.RIGHT_CURLY_BRACE || c == TagUtils.RIGHT_SQUARE_BRACE);

		private bool IsQuotationMark(char c) =>
			c == TagUtils.SLASH_WITH_QUOTATION_TAG || c == TagUtils.QUOTATION_TAG_1 || c == TagUtils.QUOTATION_TAG_2;

		private bool IsEndOfStream(char c) =>
			c == TagUtils.END_OF_STREAM_TAG;

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
					case TagUtils.END_OF_STREAM_TAG:
						throw new FormatException("[JsonReader::ReadString] String without closing quotation mark detected.");
					case TagUtils.DOUBLE_SLASH_TAG:
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
