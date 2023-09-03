namespace DataType
{
	[UnityEngine.Scripting.Preserve]
	public class ShortDataType : DataType
	{
		public ShortDataType(IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(short), dataTypeController, reflectionHelper) => IsPrimitive = true;

		public override void Write(object obj, IWriter writer) =>
			writer.WritePrimitive((short)obj);

		public override object Read<T>(IReader reader) =>
			(T)(object)reader.ReadShort();
	}

	public class ShortArrayDataType : ArrayDataType
	{
		public ShortArrayDataType(ShortDataType shortElementDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(
		                                                                                                                                                 typeof(short[]), shortElementDataType, dataTypeController, reflectionHelper) { }
	}
}
