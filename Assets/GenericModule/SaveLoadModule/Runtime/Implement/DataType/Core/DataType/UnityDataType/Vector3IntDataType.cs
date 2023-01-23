#if UNITY_2017_2_OR_NEWER
using UnityEngine;

namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    //[ES3PropertiesAttribute("x", "y", "z")]
    public class Vector3IntDataType : DataType
    {
        public static DataType Instance { get; private set; }

        public Vector3IntDataType() : base(typeof(Vector3Int)) =>
            Instance = this;


        public override void Write(object obj, IWriter writer)
        {
            var vector3Int = (Vector3Int)obj;
            writer.WriteProperty("x", vector3Int.x, IntDataType.Instance);
            writer.WriteProperty("y", vector3Int.y, IntDataType.Instance);
            writer.WriteProperty("z", vector3Int.z, IntDataType.Instance);
        }

        public override object Read<T>(IReader reader) =>
            new Vector3Int(reader.ReadProperty<int>(IntDataType.Instance),
                reader.ReadProperty<int>(IntDataType.Instance),
                reader.ReadProperty<int>(IntDataType.Instance));
    }

    public class Vector3IntArrayDataType : ArrayDataType
    {
        public static DataType Instance { get; private set; }

        public Vector3IntArrayDataType(IReflectionHelper reflectionHelper) : base(typeof(Vector3Int[]), Vector3IntDataType.Instance, reflectionHelper) =>
            Instance = this;
    }
}
#endif