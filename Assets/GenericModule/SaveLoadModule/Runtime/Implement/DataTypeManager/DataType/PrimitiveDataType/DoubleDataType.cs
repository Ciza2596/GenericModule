using System;
using UnityEngine;

namespace DataTypeManager
{
	[UnityEngine.Scripting.Preserve]
	public class DoubleDataType : DataType
	{
		public static DataType Instance = null;

		public DoubleDataType() : base(typeof(double))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, IWriter writer)
		{
			writer.WritePrimitive((double)obj);
		}

		public override object Read<T>(IReader reader)
		{
			return (T)(object)reader.Read_double();
		}
	}

	public class DoubleArrayDataType : ArrayDataType
	{
		public static DataType Instance;

		public DoubleArrayDataType() : base(typeof(double[]), DoubleDataType.Instance)
		{
			Instance = this;
		}
	}
}