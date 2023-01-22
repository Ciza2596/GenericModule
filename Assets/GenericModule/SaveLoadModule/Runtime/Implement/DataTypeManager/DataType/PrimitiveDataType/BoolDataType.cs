using System;
using UnityEngine;

namespace DataTypeManager
{
	[UnityEngine.Scripting.Preserve]
	public class BoolDataType : DataType
	{
		public static DataType Instance = null;

		public BoolDataType() : base(typeof(bool))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, IWriter writer)
		{
			writer.WritePrimitive((bool)obj);
		}

		public override object Read<T>(IReader reader)
		{
			return (T)(object)reader.Read_bool();
		}
	}

	public class BoolArrayDataType : ArrayDataType
	{
		public static DataType Instance;

		public BoolArrayDataType() : base(typeof(bool[]), BoolDataType.Instance)
		{
			Instance = this;
		}
	}
}