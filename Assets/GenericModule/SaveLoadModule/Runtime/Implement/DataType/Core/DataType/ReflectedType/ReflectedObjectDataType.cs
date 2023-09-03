using System;

namespace DataType
{
	[UnityEngine.Scripting.Preserve]
	public class ReflectedObjectDataType : ObjectType
	{
		public ReflectedObjectDataType(Type type, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(type, dataTypeController, reflectionHelper) =>
			GetProperties();

		protected override void WriteObject(object obj, IWriter writer) =>
			WriteProperties(obj, writer);

		protected override object ReadObject<T>(IReader reader)
		{
			var obj = _reflectionHelper.CreateInstance(this.Type);
			ReadProperties(reader, obj);
			return obj;
		}

		protected override void ReadObject<T>(IReader reader, object obj) =>
			ReadProperties(reader, obj);
	}
}
