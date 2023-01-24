namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class DoubleDataType : DataType
    {
        public DoubleDataType() : base(typeof(double))
        {
        }

        public override void Write(object obj, IWriter writer)
        {
            writer.WritePrimitive((double)obj);
        }

        public override object Read<T>(IReader reader) =>
            (T)(object)reader.ReadDouble();
    }

    public class DoubleArrayDataType : ArrayDataType
    {
        public DoubleArrayDataType(DoubleDataType doubleDataType, IReflectionHelper reflectionHelper) : base(
            typeof(double[]), doubleDataType, reflectionHelper)
        {
        }
    }
}