using System;

namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class EnumDataType : DataType
    {
        //private variable
        private readonly Type _underlyingType;


        //public method
        public EnumDataType(Type type, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(type, dataTypeController, reflectionHelper)
        {
            IsPrimitive = true;
            IsEnum = true;
            
            _underlyingType = Enum.GetUnderlyingType(type);
        }

        public override void Write(object obj, IWriter writer)
        {
            if (_underlyingType == typeof(bool)) writer.WritePrimitive((bool)obj);
            else if (_underlyingType == typeof(char)) writer.WritePrimitive((char)obj);
            else if (_underlyingType == typeof(double)) writer.WritePrimitive((double)obj);
            else if (_underlyingType == typeof(float)) writer.WritePrimitive((float)obj);
            else if (_underlyingType == typeof(int)) writer.WritePrimitive((int)obj);
            else if (_underlyingType == typeof(long)) writer.WritePrimitive((long)obj);
            else if (_underlyingType == typeof(short)) writer.WritePrimitive((short)obj);
            else if (_underlyingType == typeof(byte)) writer.WritePrimitive((byte)obj);
            else if (_underlyingType == typeof(decimal)) writer.WritePrimitive((decimal)obj);
            else if (_underlyingType == typeof(sbyte)) writer.WritePrimitive((sbyte)obj);
            else if (_underlyingType == typeof(uint)) writer.WritePrimitive((uint)obj);
            else if (_underlyingType == typeof(ulong)) writer.WritePrimitive((ulong)obj);
            else if (_underlyingType == typeof(ushort)) writer.WritePrimitive((ushort)obj);
            else
                throw new System.InvalidCastException("The underlying type " + _underlyingType + " of Enum " + Type +
                                                      " is not supported");
        }

        public override object Read<T>(IReader reader)
        {
            if (_underlyingType == typeof(int)) return Enum.ToObject(Type, reader.ReadInt());
            if (_underlyingType == typeof(bool)) return Enum.ToObject(Type, reader.ReadBool());
            if (_underlyingType == typeof(byte)) return Enum.ToObject(Type, reader.ReadByte());
            if (_underlyingType == typeof(char)) return Enum.ToObject(Type, reader.ReadChar());
            if (_underlyingType == typeof(decimal)) return Enum.ToObject(Type, reader.ReadDecimal());
            if (_underlyingType == typeof(double)) return Enum.ToObject(Type, reader.ReadDouble());
            if (_underlyingType == typeof(float)) return Enum.ToObject(Type, reader.ReadFloat());
            if (_underlyingType == typeof(long)) return Enum.ToObject(Type, reader.ReadLong());
            if (_underlyingType == typeof(sbyte)) return Enum.ToObject(Type, reader.ReadSbyte());
            if (_underlyingType == typeof(short)) return Enum.ToObject(Type, reader.ReadShort());
            if (_underlyingType == typeof(uint)) return Enum.ToObject(Type, reader.ReadUint());
            if (_underlyingType == typeof(ulong)) return Enum.ToObject(Type, reader.ReadUlong());
            if (_underlyingType == typeof(ushort)) return Enum.ToObject(Type, reader.ReadUshort());
            
            throw new System.InvalidCastException("The underlying type " + _underlyingType + " of Enum " + Type +
                                                      " is not supported");
        }
    }
}