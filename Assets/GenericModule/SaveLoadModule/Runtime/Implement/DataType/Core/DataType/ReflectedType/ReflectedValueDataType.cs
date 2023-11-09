using System;
using UnityEngine.Scripting;

namespace DataType
{
	internal class ReflectedValueDataType : BaseDataType
	{
		[Preserve]
		public ReflectedValueDataType(Type type, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(type, dataTypeController, reflectionHelper) =>
			GetProperties();

		public override void Write(object obj, IWriter writer) =>
			WriteProperties(obj, writer);

		public override object Read<T>(IReader reader)
		{
			var obj = _reflectionHelper.CreateInstance(Type);

			if (obj == null)
				throw new NotSupportedException($"[ReflectedValueDataType::Read] Cannot create an instance of {Type}. However, you may be able to add support for it using a custom DataType file.");

			// Make sure we return the result of ReadProperties as properties aren't assigned by reference.
			return ReadProperties(reader, obj);
		}

		public override void ReadInto<T>(IReader reader, object obj)
		{
			throw new NotSupportedException("[ReflectedValueDataType::Read] Cannot perform self-assigning load on a value type.");
		}
	}
}
