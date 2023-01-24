
namespace DataType
{
	[UnityEngine.Scripting.Preserve]
	public class ShortDataType : DataType
	{
		public ShortDataType() : base(typeof(short)) {
		}
		

		public override void Write(object obj, IWriter writer)
		{
			writer.WritePrimitive((short)obj);
		}

		public override object Read<T>(IReader reader) =>
			(T)(object)reader.ReadShort();
		
	}

	public class ShortArrayDataType : ArrayDataType
	{

		public ShortArrayDataType(ShortDataType shortDataType, IReflectionHelper reflectionHelper) : base(
			typeof(short[]), shortDataType, reflectionHelper)
		{
		}

	}
}