
namespace DataTypeManager
{
	[UnityEngine.Scripting.Preserve]
	public class LongDataType : DataType
	{
		public static DataType Instance { get; private set; }

		public LongDataType() : base(typeof(long)) =>
			Instance = this;

		public override void Write(object obj, IWriter writer)
		{
			writer.WritePrimitive((long)obj);
		}

		public override object Read<T>(IReader reader) =>
			(T)(object)reader.ReadLong();
		
	}

	public class LongArrayDataType : ArrayDataType
	{
		public static DataType Instance { get; private set; }

		public LongArrayDataType() : base(typeof(long[]), LongDataType.Instance) => 
			Instance = this;
		
	}
}