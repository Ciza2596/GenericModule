using UnityEngine.Scripting;

namespace DataType
{
	[Preserve]
	public class DoubleDataType : BaseDataType
	{
		[Preserve]
		public DoubleDataType(IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(double), dataTypeController, reflectionHelper) => IsPrimitive = true;

		public override void Write(object obj, IWriter writer) =>
			writer.WritePrimitive((double)obj);

		public override object Read<T>(IReader reader) =>
			(T)(object)reader.ReadDouble();
	}

	[Preserve]
	public class DoubleArrayDataType : ArrayDataType
	{
		[Preserve]
		public DoubleArrayDataType(DoubleDataType doubleElementDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(double[]), doubleElementDataType, dataTypeController, reflectionHelper) { }
	}
}
