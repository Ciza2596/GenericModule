
namespace DataType
{
	[UnityEngine.Scripting.Preserve]
	public class LongDataType : DataType
	{
		public LongDataType() : base(typeof(long))
		{
		}
		
		public override void Write(object obj, IWriter writer)
		{
			writer.WritePrimitive((long)obj);
		}

		public override object Read<T>(IReader reader) =>
			(T)(object)reader.ReadLong();
		
	}

	public class LongArrayDataType : ArrayDataType
	{

		public LongArrayDataType(LongDataType longDataType, IReflectionHelper reflectionHelper) : base(typeof(long[]),
			longDataType, reflectionHelper)
		{
		}
	}
}