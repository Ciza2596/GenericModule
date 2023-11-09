using UnityEngine.Scripting;

namespace DataType
{
	public class FloatDataType : BaseDataType
	{
		[Preserve]
		public FloatDataType(IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(float), dataTypeController, reflectionHelper) => IsPrimitive = true;

		public override void Write(object obj, IWriter writer) =>
			writer.WritePrimitive((float)obj);

		public override object Read<T>(IReader reader) =>
			(T)(object)reader.ReadFloat();
	}

	public class FloatArrayDataType : ArrayDataType
	{
		[Preserve]
		public FloatArrayDataType(FloatDataType floatElementDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(float[]), floatElementDataType, dataTypeController, reflectionHelper) { }
	}
}
