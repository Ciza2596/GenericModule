﻿using System;
using UnityEngine;

namespace DataTypeManager
{
	[UnityEngine.Scripting.Preserve]
	public class EnumDataType : DataType
	{
		public static DataType Instance = null;
		private Type underlyingType = null;

		public EnumDataType(Type type) : base(type)
		{
			isPrimitive = true;
            isEnum = true;
			Instance = this;
			underlyingType = Enum.GetUnderlyingType(type);
		}

		public override void Write(object obj, IWriter writer)
		{
				 if(underlyingType == typeof(int))		writer.WritePrimitive((int)obj);
			else if(underlyingType == typeof(bool))		writer.WritePrimitive((bool)obj);
			else if(underlyingType == typeof(byte))		writer.WritePrimitive((byte)obj);
			else if(underlyingType == typeof(char))		writer.WritePrimitive((char)obj);
			else if(underlyingType == typeof(decimal))	writer.WritePrimitive((decimal)obj);
			else if(underlyingType == typeof(double))	writer.WritePrimitive((double)obj);
			else if(underlyingType == typeof(float))	writer.WritePrimitive((float)obj);
			else if(underlyingType == typeof(long))		writer.WritePrimitive((long)obj);
			else if(underlyingType == typeof(sbyte))	writer.WritePrimitive((sbyte)obj);
			else if(underlyingType == typeof(short))	writer.WritePrimitive((short)obj);
			else if(underlyingType == typeof(uint))		writer.WritePrimitive((uint)obj);
			else if(underlyingType == typeof(ulong))	writer.WritePrimitive((ulong)obj);
			else if(underlyingType == typeof(ushort))	writer.WritePrimitive((ushort)obj);
			else
				throw new System.InvalidCastException("The underlying type " + underlyingType + " of Enum "+type+" is not supported");

		}

		public override object Read<T>(IReader reader)
		{
				 if(underlyingType == typeof(int))		return Enum.ToObject (type, reader.Read_int());
			else if(underlyingType == typeof(bool))		return Enum.ToObject (type, reader.Read_bool());
			else if(underlyingType == typeof(byte))		return Enum.ToObject (type, reader.Read_byte());
			else if(underlyingType == typeof(char))		return Enum.ToObject (type, reader.Read_char());
			else if(underlyingType == typeof(decimal))	return Enum.ToObject (type, reader.Read_decimal());
			else if(underlyingType == typeof(double))	return Enum.ToObject (type, reader.Read_double());
			else if(underlyingType == typeof(float))	return Enum.ToObject (type, reader.Read_float());
			else if(underlyingType == typeof(long))		return Enum.ToObject (type, reader.Read_long());
			else if(underlyingType == typeof(sbyte))	return Enum.ToObject (type, reader.Read_sbyte());
			else if(underlyingType == typeof(short))	return Enum.ToObject (type, reader.Read_short());
			else if(underlyingType == typeof(uint))		return Enum.ToObject (type, reader.Read_uint());
			else if(underlyingType == typeof(ulong))	return Enum.ToObject (type, reader.Read_ulong());
			else if(underlyingType == typeof(ushort))	return Enum.ToObject (type, reader.Read_ushort());
			else
				throw new System.InvalidCastException("The underlying type " + underlyingType + " of Enum "+type+" is not supported");
		}
	}
}