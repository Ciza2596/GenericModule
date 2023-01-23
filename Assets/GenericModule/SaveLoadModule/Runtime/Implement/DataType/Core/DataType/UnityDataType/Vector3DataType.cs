using UnityEngine;

namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    //[ES3Properties("x", "y", "z")]
    public class Vector3DataType : DataType
    {
        public static DataType Instance { get; private set; }

        public Vector3DataType() : base(typeof(Vector3)) =>
            Instance = this;


        public override void Write(object obj, IWriter writer)
        {
            var vector3 = (Vector3)obj;
            writer.WriteProperty("x", vector3.x, FloatDataType.Instance);
            writer.WriteProperty("y", vector3.y, FloatDataType.Instance);
            writer.WriteProperty("z", vector3.z, FloatDataType.Instance);
        }

        public override object Read<T>(IReader reader) =>
            new Vector3(reader.ReadProperty<float>(FloatDataType.Instance),
                reader.ReadProperty<float>(FloatDataType.Instance),
                reader.ReadProperty<float>(FloatDataType.Instance));
    }

    public class Vector3ArrayDataType : ArrayDataType
    {
        public static DataType Instance { get; private set; }

        public Vector3ArrayDataType(IReflectionHelper reflectionHelper) : base(typeof(Vector3[]), Vector3DataType.Instance, reflectionHelper) =>
            Instance = this;
    }
}