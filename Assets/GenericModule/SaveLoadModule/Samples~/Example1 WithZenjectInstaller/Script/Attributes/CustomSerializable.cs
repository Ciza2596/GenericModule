using System;

namespace SaveLoadModule.Example1
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    public class CustomSerializable : Attribute
    {
    }
}