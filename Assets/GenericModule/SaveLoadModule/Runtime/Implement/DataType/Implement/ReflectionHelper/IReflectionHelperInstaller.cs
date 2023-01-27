
using System;

namespace DataType.Implement
{
    public interface IReflectionHelperInstaller
    {
        Type CustomSerializableAttributeType { get; }
        Type CustomNonSerializableAttributeType { get; }
    }
}
