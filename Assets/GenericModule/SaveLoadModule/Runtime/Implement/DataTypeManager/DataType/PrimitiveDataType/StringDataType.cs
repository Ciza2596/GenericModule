using System;
using UnityEngine;

namespace DataTypeManager
{
	[UnityEngine.Scripting.Preserve]
	public class StringDataType : DataType
	{
		public static DataType Instance = null;

		public StringDataType() : base(typeof(string))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, IWriter writer)
		{
			writer.WritePrimitive((string)obj);
		}

		public override object Read<T>(IReader reader)
		{
			return reader.Read_string();
		}
	}
	
	
	public class StringArrayDataType : ArrayDataType
	{
		public static DataType Instance;

		public StringArrayDataType() : base(typeof(string[]), StringDataType.Instance)
		{
			Instance = this;
		}
	}
}