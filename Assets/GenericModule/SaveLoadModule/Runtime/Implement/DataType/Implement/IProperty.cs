using System;


namespace DataType
{
    public interface IProperty
    {
        //public variable
        public string Name { get; }
        public Type Type { get; }
        
        
        //public method
        public object GetValue(object obj);
    }
}
