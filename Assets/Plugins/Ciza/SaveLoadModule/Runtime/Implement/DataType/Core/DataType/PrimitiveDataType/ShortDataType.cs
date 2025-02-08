using UnityEngine.Scripting;

namespace DataType
{
	[Preserve]
	public class ShortDataType : BaseDataType
	{
		[Preserve]
		public ShortDataType(IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(short), dataTypeController, reflectionHelper) => IsPrimitive = true;

		public override void Write(object obj, IWriter writer) =>
			writer.WritePrimitive((short)obj);

		public override object Read<T>(IReader reader) =>
			(T)(object)reader.ReadShort();
	}

	[Preserve]
	public class ShortArrayDataType : ArrayDataType
	{
		[Preserve]
		public ShortArrayDataType(ShortDataType shortElementDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(
		                                                                                                                                                 typeof(short[]), shortElementDataType, dataTypeController, reflectionHelper) { }
	}
}
