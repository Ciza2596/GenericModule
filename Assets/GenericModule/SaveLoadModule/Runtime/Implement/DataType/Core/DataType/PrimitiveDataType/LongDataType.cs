using UnityEngine.Scripting;

namespace DataType
{
	public class LongDataType : BaseDataType
	{
		[Preserve]
		public LongDataType(IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(long), dataTypeController, reflectionHelper) => IsPrimitive = true;

		public override void Write(object obj, IWriter writer) =>
			writer.WritePrimitive((long)obj);

		public override object Read<T>(IReader reader) =>
			(T)(object)reader.ReadLong();
	}

	public class LongArrayDataType : ArrayDataType
	{
		[Preserve]
		public LongArrayDataType(LongDataType longElementDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(long[]),
		                                                                                                                                              longElementDataType, dataTypeController, reflectionHelper) { }
	}
}
