using System;
using UnityEngine;

namespace DataTypeManager
{
	[UnityEngine.Scripting.Preserve]
	public class LongDataType : DataType
	{
		public static DataType Instance = null;

		public LongDataType() : base(typeof(long))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, IWriter writer)
		{
			writer.WritePrimitive((long)obj);
		}

		public override object Read<T>(IReader reader)
		{
			return (T)(object)reader.Read_long();
		}
	}

	public class LongArrayDataType : ArrayDataType
	{
		public static DataType Instance;

		public LongArrayDataType() : base(typeof(long[]), LongDataType.Instance)
		{
			Instance = this;
		}
	}
}