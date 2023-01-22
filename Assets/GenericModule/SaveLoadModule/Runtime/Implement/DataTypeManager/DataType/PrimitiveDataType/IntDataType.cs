using System;
using UnityEngine;

namespace DataTypeManager
{
	[UnityEngine.Scripting.Preserve]
	public class IntDataType : DataType
	{
		public static DataType Instance = null;

		public IntDataType() : base(typeof(int))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, IWriter writer)
		{
			writer.WritePrimitive((int)obj);
		}

		public override object Read<T>(IReader reader)
		{
			return (T)(object)reader.Read_int();
		}
	}

	public class IntArrayDataType : ArrayDataType
	{
		public static DataType Instance;

		public IntArrayDataType() : base(typeof(int[]), IntDataType.Instance)
		{
			Instance = this;
		}
	}
}