

using System;

namespace DataTypeManager
{
    public interface IWriter
    {
        void WriteType(Type type);


        //property
        void WriteProperty(string name, object value, DataType dataType);
        void WriteProperty(string name, int value, DataType dataType);
        
        
        //primitive
        void WritePrimitive(int value);
        void WritePrimitive(bool value);
        void WritePrimitive(byte value);
        void WritePrimitive(char value);
        void WritePrimitive(decimal value);
        void WritePrimitive(double value);
        void WritePrimitive(float value);
        void WritePrimitive(long value);
        void WritePrimitive(sbyte value);
        void WritePrimitive(short value);
        void WritePrimitive(uint value);
        void WritePrimitive(ulong value);
        void WritePrimitive(ushort value);
    }
}
