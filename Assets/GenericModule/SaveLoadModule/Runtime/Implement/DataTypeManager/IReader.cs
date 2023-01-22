

using System;
using System.Collections.Generic;

namespace DataTypeManager
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
        
        
        string ReadPropertyName();
    }
}
