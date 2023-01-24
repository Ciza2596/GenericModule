﻿namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class BoolDataType : DataType
    {
        public BoolDataType() : base(typeof(bool))
        {
        }

        public override void Write(object obj, IWriter writer) =>
            writer.WritePrimitive((bool)obj);


        public override object Read<T>(IReader reader) =>
            (T)(object)reader.ReadBool();
    }

    public class BoolArrayDataType : ArrayDataType
    {
        public BoolArrayDataType(BoolDataType boolDataType, IReflectionHelper reflectionHelper) : base(typeof(bool[]),
            boolDataType,
            reflectionHelper)
        {
        }
    }
}