namespace DataTypeManager
{
    [UnityEngine.Scripting.Preserve]
    public class BoolDataType : DataType
    {
        public static DataType Instance { get; private set; }

        public BoolDataType() : base(typeof(bool)) =>
            Instance = this;


        public override void Write(object obj, IWriter writer) =>
            writer.WritePrimitive((bool)obj);


        public override object Read<T>(IReader reader) =>
            (T)(object)reader.ReadBool();
    }

    public class BoolArrayDataType : ArrayDataType
    {
        public static DataType Instance { get; private set; }

        public BoolArrayDataType() : base(typeof(bool[]), BoolDataType.Instance) =>
            Instance = this;
    }
}