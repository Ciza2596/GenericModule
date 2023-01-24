namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class IntDataType : DataType
    {

        public IntDataType() : base(typeof(int))
        {
        }

        public override void Write(object obj, IWriter writer)
        {
            writer.WritePrimitive((int)obj);
        }

        public override object Read<T>(IReader reader) =>
            (T)(object)reader.ReadInt();
    }

    public class IntArrayDataType : ArrayDataType
    {

        public IntArrayDataType(IntDataType intDataType, IReflectionHelper reflectionHelper) : base(typeof(int[]),
            intDataType, reflectionHelper)
        {
        }
    }
}