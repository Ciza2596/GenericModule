using System;
using UnityEngine;

namespace DataTypeManager
{
    [UnityEngine.Scripting.Preserve]
    //[ES3Properties("x", "y", "z")]
    public class Vector3DataType : DataType
    {
        public static DataType Instance = null;

        public Vector3DataType() : base(typeof(Vector3))
        {
            Instance = this;
        }

        public override void Write(object obj, IWriter writer)
        {
            Vector3 casted = (Vector3)obj;
            writer.WriteProperty("x", casted.x, FloatDataType.Instance);
            writer.WriteProperty("y", casted.y, FloatDataType.Instance);
            writer.WriteProperty("z", casted.z, FloatDataType.Instance);
        }

        public override object Read<T>(IReader reader)
        {
            return new Vector3(reader.ReadProperty<float>(FloatDataType.Instance),
                reader.ReadProperty<float>(FloatDataType.Instance),
                reader.ReadProperty<float>(FloatDataType.Instance));
        }
    }

    public class Vector3ArrayDataType : ArrayDataType
    {
        public static DataType Instance;

        public Vector3ArrayDataType() : base(typeof(Vector3[]), Vector3DataType.Instance)
        {
            Instance = this;
        }
    }
}