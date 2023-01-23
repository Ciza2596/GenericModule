#if UNITY_2017_2_OR_NEWER
using UnityEngine;

namespace DataTypeManager
{
    [UnityEngine.Scripting.Preserve]
    //[ES3PropertiesAttribute("x", "y")]
    public class Vector2IntDataType : DataType
    {
        public static DataType Instance { get; private set; }

        public Vector2IntDataType() : base(typeof(Vector2Int)) => Instance = this;


        public override void Write(object obj, IWriter writer)
        {
            var vector2Int = (Vector2Int)obj;
            writer.WriteProperty("x", vector2Int.x, IntDataType.Instance);
            writer.WriteProperty("y", vector2Int.y, IntDataType.Instance);
        }

        public override object Read<T>(IReader reader) =>
            new Vector2Int(reader.ReadProperty<int>(IntDataType.Instance),
                reader.ReadProperty<int>(IntDataType.Instance));
    }

    public class Vector2IntArrayDataType : ArrayDataType
    {
        public static DataType Instance { get; private set; }

        public Vector2IntArrayDataType() : base(typeof(Vector2Int[]), Vector2IntDataType.Instance) =>
            Instance = this;
    }
}
#endif