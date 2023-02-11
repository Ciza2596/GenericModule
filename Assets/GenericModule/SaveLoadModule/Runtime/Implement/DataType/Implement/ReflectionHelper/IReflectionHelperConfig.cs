
using System;

namespace DataType.Implement
{
    public interface IReflectionHelperConfig
    {
        Type CustomSerializableAttributeType { get; }
        Type CustomNonSerializableAttributeType { get; }
    }
}
