
namespace DataType
{
	[UnityEngine.Scripting.Preserve]
	public class StringDataType : DataType
	{
		public static DataType Instance { get; private set; }

		public StringDataType() : base(typeof(string)) =>
			Instance = this;
		

		public override void Write(object obj, IWriter writer)
		{
			writer.WritePrimitive((string)obj);
		}

		public override object Read<T>(IReader reader) =>
			reader.ReadString();
		
	}
	
	
	public class StringArrayDataType : ArrayDataType
	{
		public static DataType Instance { get; private set; }

		public StringArrayDataType(IReflectionHelper reflectionHelper) : base(typeof(string[]), StringDataType.Instance, reflectionHelper) =>
			Instance = this;
	}
}