using System;

namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class DateTimeDataType : DataType
    {
        private readonly LongDataType _longDataType;

        public DateTimeDataType(LongDataType longDataType) : base(typeof(DateTime)) =>
            _longDataType = longDataType;


        public override void Write(object obj, IWriter writer)
        {
            writer.WriteProperty("ticks", ((DateTime)obj).Ticks, _longDataType);
        }

        public override object Read<T>(IReader reader)
        {
            reader.ReadPropertyName();
            return new DateTime(reader.Read<long>(_longDataType));
        }
    }

    public class DateTimeArrayDataType : ArrayDataType
    {
        public DateTimeArrayDataType(DateTimeDataType dateTimeElementDataType, IReflectionHelper reflectionHelper) :
            base(
                typeof(DateTime[]),
                dateTimeElementDataType, reflectionHelper)
        {
        }
    }
}