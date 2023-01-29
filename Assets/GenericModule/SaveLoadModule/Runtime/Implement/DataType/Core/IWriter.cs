

using System;
using Object = UnityEngine.Object;

namespace DataType
{
    public interface IWriter
    {
        void WriteType(Type type);

        //write property
        void WriteProperty(string name, object value, DataType dataType);

        //write primitive
        void WritePrimitive(string value);
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


        void StartWriteCollection();
        void EndWriteCollection();
        void StartWriteCollectionItem(int index);
        void EndWriteCollectionItem(int index);
        void Write(object value, DataType dataType);
        void StartWriteDictionaryKey(int index);
        void EndWriteDictionaryKey(int index);
        void StartWriteDictionaryValue(int index);
        void EndWriteDictionaryValue(int index);
        void WriteNull();
        void WritePropertyByRef(string name, Object value);
        void WriteRef(Object value);
    }
}
