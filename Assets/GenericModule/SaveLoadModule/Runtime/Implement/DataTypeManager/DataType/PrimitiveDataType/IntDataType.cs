namespace DataTypeManager
{
    [UnityEngine.Scripting.Preserve]
    public class IntDataType : DataType
    {
        public static DataType Instance { get; private set; }

        public IntDataType() : base(typeof(int)) =>
            Instance = this;


        public override void Write(object obj, IWriter writer)
        {
            writer.WritePrimitive((int)obj);
        }

        public override object Read<T>(IReader reader) =>
            (T)(object)reader.ReadInt();
    }

    public class IntArrayDataType : ArrayDataType
    {
        public static DataType Instance { get; private set; }

        public IntArrayDataType(IReflectionHelper reflectionHelper) : base(typeof(int[]), IntDataType.Instance, reflectionHelper) =>
            Instance = this;
    }
}