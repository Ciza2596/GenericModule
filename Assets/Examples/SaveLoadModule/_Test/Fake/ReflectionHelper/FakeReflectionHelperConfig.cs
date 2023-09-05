using System;
using DataType.Implement;

public class FakeReflectionHelperConfig : IReflectionHelperConfig
{
	public Type[] CustomSerializableAttributeTypes    => new[] { typeof(FakeSerializable) };
	public Type[] CustomNonSerializableAttributeTypes => new[] { typeof(FakeNonSerializable) };
}
