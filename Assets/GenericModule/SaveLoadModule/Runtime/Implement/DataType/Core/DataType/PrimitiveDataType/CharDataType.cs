namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class CharDataType : DataType
    {
        public CharDataType() : base(typeof(char)) => IsPrimitive = true;

        public override void Write(object obj, IWriter writer)
        {
            writer.WritePrimitive((char)obj);
        }

        public override object Read<T>(IReader reader) =>
            (T)(object)reader.ReadChar();
    }

    public class CharArrayDataType : ArrayDataType
    {
        public CharArrayDataType(CharDataType elementDataType, IReflectionHelper reflectionHelper) : base(
            typeof(char[]), elementDataType,
            reflectionHelper)
        {
        }
    }
}