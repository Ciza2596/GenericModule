namespace DataTypeManager
{
	[UnityEngine.Scripting.Preserve]
	public class CharDataType : DataType
	{
		public static DataType Instance = null;

		public CharDataType() : base(typeof(char))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, IWriter writer)
		{
			writer.WritePrimitive((char)obj);
		}

		public override object Read<T>(IReader reader)
		{
			return (T)(object)reader.Read_char();
		}
	}
		public class CharArrayDataType : ArrayDataType
		{
			public static DataType Instance;

			public CharArrayDataType() : base(typeof(char[]), CharDataType.Instance)
			{
				Instance = this;
			}
	}
}