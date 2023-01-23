using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace DataType
{
    public class DataTypeController
    {
        //private variable
        private readonly Dictionary<Type, DataType> _typeDatas = new Dictionary<Type, DataType>();

        
        //public method
        public bool TryGetDataType(Type key, out DataType dataType)
        {
            var hasValue = _typeDatas.TryGetValue(key, out dataType);
            return hasValue;
        }

        public void Add(Type key, DataType value)
        {
            Assert.IsTrue(!_typeDatas.ContainsKey(key), $"[DataManager::Add] Already exist key: {key}.");
            _typeDatas.Add(key, value);
        }

        public void Clear() => _typeDatas.Clear();
    }
}