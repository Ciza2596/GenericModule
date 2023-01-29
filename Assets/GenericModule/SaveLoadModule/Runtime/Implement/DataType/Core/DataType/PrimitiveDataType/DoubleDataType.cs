namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class DoubleDataType : DataType
    {
        public DoubleDataType(IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(double), dataTypeController, reflectionHelper) => IsPrimitive = true;

        public override void Write(object obj, IWriter writer)
        {
            writer.WritePrimitive((double)obj);
        }

        public override object Read<T>(IReader reader) =>
            (T)(object)reader.ReadDouble();
    }

    public class DoubleArrayDataType : ArrayDataType
    {
        public DoubleArrayDataType(DoubleDataType doubleElementDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(
            typeof(double[]), doubleElementDataType, dataTypeController, reflectionHelper)
        {
        }
    }
}