using System;
using UnityEngine;

namespace DataTypeManager
{
	[UnityEngine.Scripting.Preserve]
	public class FloatDataType : DataType
	{
		public static DataType Instance = null;

		public FloatDataType() : base(typeof(float))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, IWriter writer)
		{
			writer.WritePrimitive((float)obj);
		}

		public override object Read<T>(IReader reader)
		{
			return (T)(object)reader.Read_float();
		}
	}

	public class FloatArrayDataType : ArrayDataType
	{
		public static DataType Instance;

		public FloatArrayDataType() : base(typeof(float[]), FloatDataType.Instance)
		{
			Instance = this;
		}
	}
}