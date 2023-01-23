namespace DataTypeManager
{
    [UnityEngine.Scripting.Preserve]
    public class CharDataType : DataType
    {
        public static DataType Instance { get; private set; }

        public CharDataType() : base(typeof(char)) =>
            Instance = this;


        public override void Write(object obj, IWriter writer)
        {
            writer.WritePrimitive((char)obj);
        }

        public override object Read<T>(IReader reader) =>
            (T)(object)reader.ReadChar();
    }

    public class CharArrayDataType : ArrayDataType
    {
        public static DataType Instance { get; private set; }

        public CharArrayDataType(IReflectionHelper reflectionHelper) : base(typeof(char[]), CharDataType.Instance,
            reflectionHelper) =>
            Instance = this;
    }
}