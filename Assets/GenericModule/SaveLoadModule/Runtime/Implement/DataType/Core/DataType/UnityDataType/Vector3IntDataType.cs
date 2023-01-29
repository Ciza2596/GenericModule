using UnityEngine;

namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class Vector3IntDataType : DataType
    {
        private IntDataType _intDataType;

        public Vector3IntDataType(IntDataType intDataType, IDataTypeController dataTypeController,
            IReflectionHelper reflectionHelper) : base(typeof(Vector3Int), dataTypeController, reflectionHelper) =>
            _intDataType = intDataType;


        public override void Write(object obj, IWriter writer)
        {
            var vector3Int = (Vector3Int)obj;
            writer.WriteProperty("x", vector3Int.x, _intDataType);
            writer.WriteProperty("y", vector3Int.y, _intDataType);
            writer.WriteProperty("z", vector3Int.z, _intDataType);
        }

        public override object Read<T>(IReader reader) =>
            new Vector3Int(reader.ReadProperty<int>(_intDataType),
                reader.ReadProperty<int>(_intDataType),
                reader.ReadProperty<int>(_intDataType));
    }

    public class Vector3IntArrayDataType : ArrayDataType
    {
        public Vector3IntArrayDataType(Vector3IntDataType vector3IntElementDataType,
            IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) :
            base(typeof(Vector3Int[]), vector3IntElementDataType, dataTypeController, reflectionHelper)
        {
        }
    }
}