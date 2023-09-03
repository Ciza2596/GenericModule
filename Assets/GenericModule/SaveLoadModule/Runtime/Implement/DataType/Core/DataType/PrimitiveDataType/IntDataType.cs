namespace DataType
{
	[UnityEngine.Scripting.Preserve]
	public class IntDataType : DataType
	{
		public IntDataType(IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(int), dataTypeController, reflectionHelper) => IsPrimitive = true;

		public override void Write(object obj, IWriter writer) =>
			writer.WritePrimitive((int)obj);

		public override object Read<T>(IReader reader) =>
			(T)(object)reader.ReadInt();
	}

	public class IntArrayDataType : ArrayDataType
	{
		public IntArrayDataType(IntDataType intElementDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(int[]),
		                                                                                                                                           intElementDataType, dataTypeController, reflectionHelper) { }
	}
}
