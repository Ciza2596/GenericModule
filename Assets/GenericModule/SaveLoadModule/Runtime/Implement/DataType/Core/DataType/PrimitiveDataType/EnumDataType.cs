using System;
using UnityEngine.Scripting;

namespace DataType
{
	public class EnumDataType : BaseDataType
	{
		//private variable
		private readonly Type _underlyingType;

		//public method
		[Preserve]
		public EnumDataType(Type type, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(type, dataTypeController, reflectionHelper)
		{
			IsPrimitive = true;
			IsEnum      = true;

			_underlyingType = Enum.GetUnderlyingType(type);
		}

		public override void Write(object obj, IWriter writer)
		{
			if (_underlyingType == TagUtils.BoolType)
				writer.WritePrimitive((bool)obj);

			else if (_underlyingType == TagUtils.CharType)
				writer.WritePrimitive((char)obj);

			else if (_underlyingType == TagUtils.DoubleType)
				writer.WritePrimitive((double)obj);

			else if (_underlyingType == TagUtils.FloatType)
				writer.WritePrimitive((float)obj);

			else if (_underlyingType == TagUtils.IntType)
				writer.WritePrimitive((int)obj);

			else if (_underlyingType == TagUtils.LongType)
				writer.WritePrimitive((long)obj);

			else if (_underlyingType == TagUtils.ShortType)
				writer.WritePrimitive((short)obj);

			else if (_underlyingType == TagUtils.ByteType)
				writer.WritePrimitive((byte)obj);

			else if (_underlyingType == TagUtils.DecimalType)
				writer.WritePrimitive((decimal)obj);

			else if (_underlyingType == TagUtils.SbyteType)
				writer.WritePrimitive((sbyte)obj);

			else if (_underlyingType == TagUtils.UintType)
				writer.WritePrimitive((uint)obj);

			else if (_underlyingType == TagUtils.UlongType)
				writer.WritePrimitive((ulong)obj);

			else if (_underlyingType == TagUtils.UshortType)
				writer.WritePrimitive((ushort)obj);

			else
				throw new InvalidCastException($"[EnumDataType::Write] The underlying type: {_underlyingType} of Enum {Type} is not supported.");
		}

		public override object Read<T>(IReader reader)
		{
			if (_underlyingType == TagUtils.IntType)
				return Enum.ToObject(Type, reader.ReadInt());

			if (_underlyingType == TagUtils.BoolType)
				return Enum.ToObject(Type, reader.ReadBool());

			if (_underlyingType == TagUtils.ByteType)
				return Enum.ToObject(Type, reader.ReadByte());

			if (_underlyingType == TagUtils.CharType)
				return Enum.ToObject(Type, reader.ReadChar());

			if (_underlyingType == TagUtils.DecimalType)
				return Enum.ToObject(Type, reader.ReadDecimal());

			if (_underlyingType == TagUtils.DoubleType)
				return Enum.ToObject(Type, reader.ReadDouble());

			if (_underlyingType == TagUtils.FloatType)
				return Enum.ToObject(Type, reader.ReadFloat());

			if (_underlyingType == TagUtils.LongType)
				return Enum.ToObject(Type, reader.ReadLong());

			if (_underlyingType == TagUtils.SbyteType)
				return Enum.ToObject(Type, reader.ReadSbyte());

			if (_underlyingType == TagUtils.ShortType)
				return Enum.ToObject(Type, reader.ReadShort());

			if (_underlyingType == TagUtils.UintType)
				return Enum.ToObject(Type, reader.ReadUint());

			if (_underlyingType == TagUtils.UlongType)
				return Enum.ToObject(Type, reader.ReadUlong());

			if (_underlyingType == TagUtils.UshortType)
				return Enum.ToObject(Type, reader.ReadUshort());

			throw new System.InvalidCastException($"The underlying type: {_underlyingType} of Enum {Type} is not supported.");
		}
	}
}
