using System;

namespace DataTypeManager
{
    public class FieldProperty : IProperty
    {
        public string Name { get; }
        public Type Type { get; }

        public object GetValue(object obj) => throw new NotImplementedException();
    }
}