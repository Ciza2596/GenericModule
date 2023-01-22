using System;
using UnityEngine;

namespace DataTypeManager
{
	[UnityEngine.Scripting.Preserve]
	public class ShortDataType : DataType
	{
		public static DataType Instance = null;

		public ShortDataType() : base(typeof(short))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, IWriter writer)
		{
			writer.WritePrimitive((short)obj);
		}

		public override object Read<T>(IReader reader)
		{
			return (T)(object)reader.Read_short();
		}
	}

	public class ShortArrayDataType : ArrayDataType
	{
		public static DataType Instance;

		public ShortArrayDataType() : base(typeof(short[]), ShortDataType.Instance)
		{
			Instance = this;
		}
	}
}