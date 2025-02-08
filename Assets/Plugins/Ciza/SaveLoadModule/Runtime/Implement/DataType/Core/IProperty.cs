

using System;

namespace DataType
{
    public interface IProperty
    {
        public string Name { get; }
        public Type Type { get; }
        public bool IsNull { get; }
        public bool IsPublic { get; }
        public bool IsStatic { get; }

        
        void SetValue(Object obj, Object value);
        Object GetValue(Object obj);
    }
}
