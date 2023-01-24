namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class CharDataType : DataType
    {
        public CharDataType() : base(typeof(char)) { }

        public override void Write(object obj, IWriter writer)
        {
            writer.WritePrimitive((char)obj);
        }

        public override object Read<T>(IReader reader) =>
            (T)(object)reader.ReadChar();
    }

    public class CharArrayDataType : ArrayDataType
    {

        public CharArrayDataType(CharDataType dataType, IReflectionHelper reflectionHelper) : base(typeof(char[]), dataType,
            reflectionHelper) { }
    }
}