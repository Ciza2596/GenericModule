
using System;

namespace DataType.Implement
{
    public interface IReflectionHelperInstaller
    {
        Type SerializableAttributeType { get; }
        Type NonSerializableAttributeType { get; }
    }
}
