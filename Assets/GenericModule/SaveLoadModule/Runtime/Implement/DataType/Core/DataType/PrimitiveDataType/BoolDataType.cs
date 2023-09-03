namespace DataType
{
	[UnityEngine.Scripting.Preserve]
	public class BoolDataType : DataType
	{
		public BoolDataType(IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(bool), dataTypeController, reflectionHelper) => IsPrimitive = true;

		public override void Write(object obj, IWriter writer) =>
			writer.WritePrimitive((bool)obj);

		public override object Read<T>(IReader reader) =>
			(T)(object)reader.ReadBool();
	}

	public class BoolArrayDataType : ArrayDataType
	{
		public BoolArrayDataType(BoolDataType boolElementDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(bool[]),
		                                                                                                                                              boolElementDataType, dataTypeController, reflectionHelper) { }
	}
}
