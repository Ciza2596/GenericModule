using System;
using System.Reflection;

namespace DataType.Implement
{
    public class Field : IProperty
    {
        //private variable
        private readonly FieldInfo _fieldInfo;
        

        //constructor
        public Field(FieldInfo fieldInfo) =>
            _fieldInfo = fieldInfo;
        

        //public variable
        public string Name => _fieldInfo.Name;
        public Type Type => _fieldInfo.FieldType;
        public bool IsNull => _fieldInfo == null;
        public bool IsPublic => _fieldInfo.IsPublic;
        public bool IsStatic => _fieldInfo.IsStatic;
        
        
        //public method
        public void SetValue(object obj, object value) =>
            _fieldInfo.SetValue(obj, value);

        public object GetValue(object obj) =>
            _fieldInfo.GetValue(obj);
        
    }
}
