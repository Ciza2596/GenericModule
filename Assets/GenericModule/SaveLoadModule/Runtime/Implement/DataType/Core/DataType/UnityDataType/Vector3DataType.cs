using UnityEngine;

namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class Vector3DataType : DataType
    {
        private readonly FloatDataType _floatDataType;

        public Vector3DataType(FloatDataType floatDataType) : base(typeof(Vector3)) =>
            _floatDataType = floatDataType;


        public override void Write(object obj, IWriter writer)
        {
            var vector3 = (Vector3)obj;
            writer.WriteProperty("x", vector3.x, _floatDataType);
            writer.WriteProperty("y", vector3.y, _floatDataType);
            writer.WriteProperty("z", vector3.z, _floatDataType);
        }

        public override object Read<T>(IReader reader) =>
            new Vector3(reader.ReadProperty<float>(_floatDataType),
                reader.ReadProperty<float>(_floatDataType),
                reader.ReadProperty<float>(_floatDataType));
    }

    public class Vector3ArrayDataType : ArrayDataType
    {

        public Vector3ArrayDataType(Vector3DataType vector3DataType, IReflectionHelper reflectionHelper) : base(
            typeof(Vector3[]), vector3DataType, reflectionHelper)
        {
        }
    }
}