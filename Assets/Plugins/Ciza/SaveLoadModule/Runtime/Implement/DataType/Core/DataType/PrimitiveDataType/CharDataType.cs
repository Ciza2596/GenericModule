using UnityEngine.Scripting;

namespace DataType
{
	[Preserve]
	public class CharDataType : BaseDataType
	{
		[Preserve]
		public CharDataType(IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(char), dataTypeController, reflectionHelper) => IsPrimitive = true;

		public override void Write(object obj, IWriter writer) =>
			writer.WritePrimitive((char)obj);

		public override object Read<T>(IReader reader) =>
			(T)(object)reader.ReadChar();
	}

	[Preserve]
	public class CharArrayDataType : ArrayDataType
	{
		[Preserve]
		public CharArrayDataType(CharDataType elementDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(char[]), elementDataType, dataTypeController, reflectionHelper) { }
	}
}
