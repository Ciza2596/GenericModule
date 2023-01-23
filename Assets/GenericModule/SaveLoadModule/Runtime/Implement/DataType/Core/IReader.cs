

using System;
using System.Collections.Generic;

namespace DataType
{
    public interface IReader
    {
        void Skip();
        IEnumerable<string> Properties { get; }

        string OverridePropertiesName { get; }


        Type ReadType();
        
        
        //primitive
        int ReadInt();
        bool ReadBool();
        byte ReadByte();
        char ReadChar();
        decimal ReadDecimal();
        double ReadDouble();
        float ReadFloat();
        long ReadLong();
        sbyte ReadSbyte();
        short ReadShort();
        uint ReadUint();
        ulong ReadUlong();
        ushort ReadUshort();
        string ReadString();
        void ReadInto<T>(T item, DataType dataType);
        
        
        string ReadPropertyName();
        T ReadProperty<T>(DataType dataType);
        T Read<T>(DataType dataType);
        
        
        
        bool StartReadCollection();
        bool StartReadCollectionItem();
        bool EndReadCollectionItem();
        void EndReadCollection();
        bool StartReadDictionary();
        bool StartReadDictionaryKey();
        void EndReadDictionaryKey();
        void StartReadDictionaryValue();
        bool EndReadDictionaryValue();
        void EndReadDictionary();
    }
}
