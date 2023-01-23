using System;

namespace DataTypeManager
{
    [UnityEngine.Scripting.Preserve]
    public class DateTimeDataType : DataType
    {
        public static DataType Instance { get; private set; }

        public DateTimeDataType() : base(typeof(DateTime)) =>
            Instance = this;


        public override void Write(object obj, IWriter writer)
        {
            writer.WriteProperty("ticks", ((DateTime)obj).Ticks, LongDataType.Instance);
        }

        public override object Read<T>(IReader reader)
        {
            reader.ReadPropertyName();
            return new DateTime(reader.Read<long>(LongDataType.Instance));
        }
    }

    public class DateTimeArrayDataType : ArrayDataType
    {
        public static DataType Instance { get; private set; }

        public DateTimeArrayDataType(IReflectionHelper reflectionHelper) : base(typeof(DateTime[]),
            DateTimeDataType.Instance, reflectionHelper) =>
            Instance = this;
    }
}