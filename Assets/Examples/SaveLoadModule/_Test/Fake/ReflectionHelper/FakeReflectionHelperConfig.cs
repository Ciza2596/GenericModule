using System;
using DataType.Implement;


public class FakeReflectionHelperConfig : IReflectionHelperConfig
{
    public Type CustomSerializableAttributeType => typeof(FakeSerializable);
    public Type CustomNonSerializableAttributeType => typeof(FakeNonSerializable);
}