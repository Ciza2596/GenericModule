namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class IntDataType : DataType
    {

        public IntDataType() : base(typeof(int)) => IsPrimitive = true;

        public override void Write(object obj, IWriter writer)
        {
            writer.WritePrimitive((int)obj);
        }

        public override object Read<T>(IReader reader) =>
            (T)(object)reader.ReadInt();
    }

    public class IntArrayDataType : ArrayDataType
    {

        public IntArrayDataType(IntDataType intElementDataType, IReflectionHelper reflectionHelper) : base(typeof(int[]),
            intElementDataType, reflectionHelper)
        {
        }
    }
}