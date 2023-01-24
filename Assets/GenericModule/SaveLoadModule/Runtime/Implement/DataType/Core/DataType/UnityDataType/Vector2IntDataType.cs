using UnityEngine;

namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class Vector2IntDataType : DataType
    {
        private readonly IntDataType _intDataType;

        public Vector2IntDataType(IntDataType intDataType) : base(typeof(Vector2Int)) => _intDataType = intDataType;


        public override void Write(object obj, IWriter writer)
        {
            var vector2Int = (Vector2Int)obj;
            writer.WriteProperty("x", vector2Int.x, _intDataType);
            writer.WriteProperty("y", vector2Int.y, _intDataType);
        }

        public override object Read<T>(IReader reader) =>
            new Vector2Int(reader.ReadProperty<int>(_intDataType),
                reader.ReadProperty<int>(_intDataType));
    }

    public class Vector2IntArrayDataType : ArrayDataType
    {
        public Vector2IntArrayDataType(Vector2IntDataType vector2IntElementDataType, IReflectionHelper reflectionHelper) :
            base(typeof(Vector2Int[]), vector2IntElementDataType, reflectionHelper)
        {
        }
    }
}
