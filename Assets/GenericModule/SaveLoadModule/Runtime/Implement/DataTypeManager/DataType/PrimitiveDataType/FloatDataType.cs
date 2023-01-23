namespace DataTypeManager
{
    [UnityEngine.Scripting.Preserve]
    public class FloatDataType : DataType
    {
        public static DataType Instance { get; private set; }

        public FloatDataType() : base(typeof(float)) =>
            Instance = this;


        public override void Write(object obj, IWriter writer)
        {
            writer.WritePrimitive((float)obj);
        }

        public override object Read<T>(IReader reader) =>
            (T)(object)reader.ReadFloat();
    }

    public class FloatArrayDataType : ArrayDataType
    {
        public static DataType Instance { get; private set; }

        public FloatArrayDataType() : base(typeof(float[]), FloatDataType.Instance) =>
            Instance = this;
    }
}