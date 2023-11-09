using UnityEngine.Scripting;

namespace DataType
{
	public class StringDataType : BaseDataType
	{
		[Preserve]
		public StringDataType(IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(
		                                                                                                         typeof(string), dataTypeController, reflectionHelper) => IsPrimitive = true;

		public override void Write(object obj, IWriter writer) =>
			writer.WritePrimitive((string)obj);

		public override object Read<T>(IReader reader) =>
			reader.ReadString();
	}

	public class StringArrayDataType : ArrayDataType
	{
		[Preserve]
		public StringArrayDataType(StringDataType stringElementDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(
		                                                                                                                                                    typeof(string[]), stringElementDataType, dataTypeController, reflectionHelper) { }
	}
}
