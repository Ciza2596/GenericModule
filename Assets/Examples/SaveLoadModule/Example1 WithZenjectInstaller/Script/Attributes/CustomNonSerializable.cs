using System;

namespace CizaSaveLoadModule.Example1
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    public class CustomNonSerializable : Attribute
    {
    }
}