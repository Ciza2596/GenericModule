

using System;
using System.Collections.Generic;
using System.Linq;

namespace CizaObjectPoolModule
{
    public class ObjectPoolModule<T> where T: class, new()
    {

        //private method
        private readonly List<T> _usingObjs = new();
        private readonly List<T> _unusingObjs = new();

        
        //public variable
        public bool HasPool => _usingObjs.Count > 0 || _unusingObjs.Count > 0;

        public int UsingCount => _usingObjs.Count;
        public int UnusingCount => _unusingObjs.Count;
        
        
        //public method
        public void Release()
        {
            var usingObjs = _usingObjs.ToArray();

            foreach (var usingObj in usingObjs)
                DeSpawn(usingObj);

            _unusingObjs.Clear();
        }

        public T Spawn()
        {
            if(_unusingObjs.Count <= 0)
                CreateObj();

            var obj = _unusingObjs.First();
            _unusingObjs.Remove(obj);
            
            _usingObjs.Add(obj);
            return obj;
        }


        public void DeSpawn(T obj)
        {
            if(obj is IDisposable disposable)
                disposable.Dispose();

            _usingObjs.Remove(obj);
            _unusingObjs.Add(obj);
        }

        //private method
        private void CreateObj()
        {
            var obj = new T();
            _unusingObjs.Add(obj);
        }

    }
}
