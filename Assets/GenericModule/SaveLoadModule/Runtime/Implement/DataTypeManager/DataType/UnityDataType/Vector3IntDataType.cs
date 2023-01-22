#if UNITY_2017_2_OR_NEWER
using System;
using UnityEngine;

namespace DataTypeManager
{
    [UnityEngine.Scripting.Preserve]
    //[ES3PropertiesAttribute("x", "y", "z")]
    public class Vector3IntDataType : DataType
    {
        public static DataType Instance = null;

        public Vector3IntDataType() : base(typeof(Vector3Int))
        {
            Instance = this;
        }

        public override void Write(object obj, IWriter writer)
        {
            Vector3Int casted = (Vector3Int)obj;
            writer.WriteProperty("x", casted.x, IntDataType.Instance);
            writer.WriteProperty("y", casted.y, IntDataType.Instance);
            writer.WriteProperty("z", casted.z, IntDataType.Instance);
        }

        public override object Read<T>(IReader reader)
        {
            return new Vector3Int(reader.ReadProperty<int>(IntDataType.Instance),
                reader.ReadProperty<int>(IntDataType.Instance),
                reader.ReadProperty<int>(IntDataType.Instance));
        }
    }

    public class Vector3IntArrayDataType : ArrayDataType
    {
        public static DataType Instance;

        public Vector3IntArrayDataType() : base(typeof(Vector3Int[]), Vector3IntDataType.Instance)
        {
            Instance = this;
        }
    }
}
#endif