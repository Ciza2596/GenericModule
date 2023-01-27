using System;
using System.Reflection;

namespace DataType.Implement
{
    public class Property : IProperty
    {
        //private variable
        private readonly PropertyInfo _propertyInfo;


        //constructor
        public Property(PropertyInfo propertyInfo) => _propertyInfo = propertyInfo;


        //public variable
        public string Name => _propertyInfo.Name;
        public Type Type => _propertyInfo.PropertyType;
        public bool IsNull => _propertyInfo == null;
        public bool IsPublic => _propertyInfo.GetGetMethod(true).IsPublic;
        public bool IsStatic => _propertyInfo.GetGetMethod(true).IsStatic;


        //public method
        public void SetValue(object obj, object value) => _propertyInfo.SetValue(obj, value, null);

        public object GetValue(object obj) => _propertyInfo.GetValue(obj, null);
    }
}