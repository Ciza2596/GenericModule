using System;

namespace DataType.Implement
{
	public interface IReflectionHelperConfig
	{
		Type[] CustomSerializableAttributeTypes    { get; }
		Type[] CustomNonSerializableAttributeTypes { get; }
	}
}
