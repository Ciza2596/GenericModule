

using System;

namespace DataType
{
    public interface IWriter
    {
        void WriteType(Type type);


        //property
        void WriteProperty(string name, object value, DataType dataType);
        void WriteProperty(string name, int value, DataType dataType);
        
        
        //primitive
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
        
        
        
        void StartWriteCollectionItem(int index);
        void StartWriteCollection();
        void EndWriteCollectionItem(int index);
        void EndWriteCollection();
        void Write(object getValue, DataType dataType, ReferenceModes referenceMode);
        void StartWriteDictionaryKey(int i);
        void EndWriteDictionaryKey(int i);
        void StartWriteDictionaryValue(int i);
        void EndWriteDictionaryValue(int i);
        void WriteNull();
    }
}
