using System;
using System.Collections.Generic;

namespace DataType
{
    public interface IReader
    {
        IEnumerable<string> PropertyNames { get; }

        string OverridePropertyName { get; }

        public void SetOverridePropertyName(string overridePropertyName);

        void Skip();

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
        void ReadInto<T>(object obj, BaseDataType dataType);
        
        
        string ReadPropertyName();
        T ReadProperty<T>(BaseDataType dataType);
        T Read<T>(BaseDataType dataType);
        
        
        
        bool StartReadCollection();
        void EndReadCollection();
        
        bool StartReadCollectionItem();
        bool EndReadCollectionItem();
        
        bool StartReadDictionary();
        void EndReadDictionary();
        
        bool StartReadDictionaryKey();
        void EndReadDictionaryKey();
        
        void StartReadDictionaryValue();
        bool EndReadDictionaryValue();
    }
}
