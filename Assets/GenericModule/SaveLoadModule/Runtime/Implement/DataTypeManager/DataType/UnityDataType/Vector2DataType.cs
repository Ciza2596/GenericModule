using UnityEngine;

namespace DataTypeManager
{
    [UnityEngine.Scripting.Preserve]
    //[ES3PropertiesAttribute("x", "y")]
    public class Vector2DataType : DataType
    {
        public static DataType Instance = null;

        public Vector2DataType() : base(typeof(Vector2))
        {
            Instance = this;
        }

        public override void Write(object obj, IWriter writer)
        {
            Vector2 casted = (Vector2)obj;
            writer.WriteProperty("x", casted.x, FloatDataType.Instance);
            writer.WriteProperty("y", casted.y, FloatDataType.Instance);
        }

        public override object Read<T>(IReader reader)
        {
            return new Vector2(reader.ReadProperty<float>(FloatDataType.Instance),
                reader.ReadProperty<float>(FloatDataType.Instance));
        }
    }

    public class Vector2ArrayDataType : ArrayDataType
    {
        public static DataType Instance;

        public Vector2ArrayDataType() : base(typeof(Vector2[]), Vector2DataType.Instance)
        {
            Instance = this;
        }
    }
}