using System;
using UnityEngine.Scripting;

namespace DataType
{
	public class DateTimeDataType : BaseDataType
	{
		private readonly LongDataType _longDataType;

		[Preserve]
		public DateTimeDataType(LongDataType longDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(DateTime), dataTypeController, reflectionHelper) =>
			_longDataType = longDataType;

		public override void Write(object obj, IWriter writer) =>
			writer.WriteProperty(TagUtils.TICKS_TAG, ((DateTime)obj).Ticks, _longDataType);

		public override object Read<T>(IReader reader)
		{
			reader.ReadPropertyName();
			return new DateTime(reader.Read<long>(_longDataType));
		}
	}

	public class DateTimeArrayDataType : ArrayDataType
	{
		[Preserve]
		public DateTimeArrayDataType(DateTimeDataType dateTimeElementDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(DateTime[]),
		                                                                                                                                                          dateTimeElementDataType, dataTypeController, reflectionHelper) { }
	}
}
