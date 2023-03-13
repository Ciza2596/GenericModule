using System;
using DataType.Implement;

namespace CizaSaveLoadModule.Example1
{
    public class ReflectionHelperConfig : IReflectionHelperConfig
    {
        public Type CustomSerializableAttributeType => typeof(CustomSerializable);
        public Type CustomNonSerializableAttributeType => typeof(CustomNonSerializable);
    }
}