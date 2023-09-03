namespace DataType
{
	[UnityEngine.Scripting.Preserve]
	public class LongDataType : DataType
	{
		public LongDataType(IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(long), dataTypeController, reflectionHelper) => IsPrimitive = true;

		public override void Write(object obj, IWriter writer) =>
			writer.WritePrimitive((long)obj);

		public override object Read<T>(IReader reader) =>
			(T)(object)reader.ReadLong();
	}

	public class LongArrayDataType : ArrayDataType
	{
		public LongArrayDataType(LongDataType longElementDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(long[]),
		                                                                                                                                              longElementDataType, dataTypeController, reflectionHelper) { }
	}
}
