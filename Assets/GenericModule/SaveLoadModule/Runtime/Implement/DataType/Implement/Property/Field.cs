using System;

namespace DataType
{
    public class Field : IProperty
    {
        public string Name { get; }
        public Type Type { get; }

        public object GetValue(object obj) => throw new NotImplementedException();
    }
}