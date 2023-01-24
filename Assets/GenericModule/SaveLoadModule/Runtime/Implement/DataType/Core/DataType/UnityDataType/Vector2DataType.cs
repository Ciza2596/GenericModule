using UnityEngine;

namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class Vector2DataType : DataType
    {
        private readonly FloatDataType _floatDataType;

        public Vector2DataType(FloatDataType floatDataType) : base(typeof(Vector2)) => _floatDataType = floatDataType;

        public override void Write(object obj, IWriter writer)
        {
            var vector2 = (Vector2)obj;
            writer.WriteProperty("x", vector2.x, _floatDataType);
            writer.WriteProperty("y", vector2.y, _floatDataType);
        }

        public override object Read<T>(IReader reader) =>
            new Vector2(reader.ReadProperty<float>(_floatDataType),
                reader.ReadProperty<float>(_floatDataType));
    }

    public class Vector2ArrayDataType : ArrayDataType
    {
        public Vector2ArrayDataType(Vector2DataType vector2DataType, IReflectionHelper reflectionHelper) : base(
            typeof(Vector2[]), vector2DataType, reflectionHelper)
        {
        }
    }
}