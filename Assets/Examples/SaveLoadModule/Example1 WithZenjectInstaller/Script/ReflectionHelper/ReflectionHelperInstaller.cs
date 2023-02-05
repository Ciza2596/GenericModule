using System;
using DataType.Implement;

namespace SaveLoadModule.Example1
{
    public class ReflectionHelperInstaller : IReflectionHelperInstaller
    {
        public Type CustomSerializableAttributeType => typeof(CustomSerializable);
        public Type CustomNonSerializableAttributeType => typeof(CustomNonSerializable);
    }
}