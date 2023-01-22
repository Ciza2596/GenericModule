
using System;

namespace DataTypeManager
{
    public class PropertyProperty: IProperty
    {
        public string Name { get; }
        public Type Type { get; }
        
        public object GetValue(object obj) => throw new NotImplementedException();
    }
}
