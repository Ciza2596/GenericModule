namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class FloatDataType : DataType
    {

        public FloatDataType() : base(typeof(float))
        {
        }

        public override void Write(object obj, IWriter writer)
        {
            writer.WritePrimitive((float)obj);
        }

        public override object Read<T>(IReader reader) =>
            (T)(object)reader.ReadFloat();
    }

    public class FloatArrayDataType : ArrayDataType
    {

        public FloatArrayDataType(FloatDataType floatDataType, IReflectionHelper reflectionHelper) : base(typeof(float[]), floatDataType,
            reflectionHelper)
        {
        }

    }
}