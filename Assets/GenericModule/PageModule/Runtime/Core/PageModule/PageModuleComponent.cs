using System;
using UnityEngine;

namespace CizaPageModule
{
    internal class PageModuleComponent : MonoBehaviour
    {
        //private variable
        private Action<float> _updateCallback;
        private Action<float> _fixedUpdateCallback;
        private Action _applicationQuit;

        

        //unity callback
        private void Update() =>
            _updateCallback?.Invoke(Time.deltaTime);
        
        private void FixedUpdate() =>
            _fixedUpdateCallback?.Invoke(Time.fixedDeltaTime);
        


        //public method
        public void SetUpdateCallback(Action<float> updateCallback) =>
            _updateCallback = updateCallback;

        public void SetFixedUpdateCallback(Action<float> fixedUpdateCallback) =>
            _fixedUpdateCallback = fixedUpdateCallback;
    }
}