
namespace DataType
{
	[UnityEngine.Scripting.Preserve]
	public class StringDataType : DataType
	{

		public StringDataType() : base(typeof(string))
		{
		}


		public override void Write(object obj, IWriter writer)
		{
			writer.WritePrimitive((string)obj);
		}

		public override object Read<T>(IReader reader) =>
			reader.ReadString();
		
	}
	
	
	public class StringArrayDataType : ArrayDataType
	{
		public StringArrayDataType(StringDataType stringDataType, IReflectionHelper reflectionHelper) : base(
			typeof(string[]), stringDataType, reflectionHelper)
		{
		}
	}
}