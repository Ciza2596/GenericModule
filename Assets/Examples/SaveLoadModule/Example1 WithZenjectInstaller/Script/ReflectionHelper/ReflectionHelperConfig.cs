using System;
using DataType.Implement;

namespace CizaSaveLoadModule.Example1
{
	public class ReflectionHelperConfig : IReflectionHelperConfig
	{
		public Type[] CustomSerializableAttributeTypes    => new[] { typeof(CustomSerializable) };
		public Type[] CustomNonSerializableAttributeTypes => new[] { typeof(CustomNonSerializable) };
	}
}
