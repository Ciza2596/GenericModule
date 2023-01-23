
namespace DataTypeManager
{
	[UnityEngine.Scripting.Preserve]
	public class ShortDataType : DataType
	{
		public static DataType Instance { get; private set; }

		public ShortDataType() : base(typeof(short)) =>
			Instance = this;
		

		public override void Write(object obj, IWriter writer)
		{
			writer.WritePrimitive((short)obj);
		}

		public override object Read<T>(IReader reader) =>
			(T)(object)reader.ReadShort();
		
	}

	public class ShortArrayDataType : ArrayDataType
	{
		public static DataType Instance { get; private set; }

		public ShortArrayDataType() : base(typeof(short[]), ShortDataType.Instance) =>
			Instance = this;
		
	}
}