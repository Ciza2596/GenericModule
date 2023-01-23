using System;

namespace DataTypeManager
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    public class ES3NonSerializable : Attribute { }
}