namespace DataTypeManager
{
    [UnityEngine.Scripting.Preserve]
    public class DoubleDataType : DataType
    {
        public static DataType Instance { get; private set; }

        public DoubleDataType() : base(typeof(double)) =>
            Instance = this;


        public override void Write(object obj, IWriter writer)
        {
            writer.WritePrimitive((double)obj);
        }

        public override object Read<T>(IReader reader) =>
            (T)(object)reader.ReadDouble();
    }

    public class DoubleArrayDataType : ArrayDataType
    {
        public static DataType Instance { get; private set; }

        public DoubleArrayDataType(IReflectionHelper reflectionHelper) : base(typeof(double[]), DoubleDataType.Instance, reflectionHelper) =>
            Instance = this;
    }
}