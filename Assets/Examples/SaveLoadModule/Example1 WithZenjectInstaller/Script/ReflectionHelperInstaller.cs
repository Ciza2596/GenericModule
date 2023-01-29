using System;
using DataType.Implement;

public class ReflectionHelperInstaller : IReflectionHelperInstaller
{
    public Type CustomSerializableAttributeType => typeof(CustomSerializable);
    public Type CustomNonSerializableAttributeType => typeof(CustomNonSerializable);
}