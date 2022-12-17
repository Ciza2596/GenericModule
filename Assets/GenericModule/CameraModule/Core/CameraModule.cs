using System.Collections.Generic;
using UnityEngine;

namespace CameraModule
{
    public class CameraModule
    {
        //private variable
        private Dictionary<string, Camera> _cameras = new Dictionary<string, Camera>();


        //public variable
        public IReadOnlyDictionary<string, Camera> Cameras => _cameras;
        


        //public method

        public void Register(string cameraName, Camera camera)
        {
            if (_cameras.ContainsKey(cameraName))
                return;
            
            _cameras.Add(cameraName, camera);
        }

        public void SetCameraData(string cameraName, int depth, LayerMask layerMask)
        {
            var isGetCamera = TryGetCamera(cameraName, out var camera);
            if (isGetCamera)
            {
                camera.depth = depth;
                camera.cullingMask = layerMask;
            }
        }

        public void OpenCamera(string cameraName)
        {
            var isGetCamera = TryGetCamera(cameraName, out var camera);
            if (isGetCamera)
                camera.enabled = true;
        }

        public void CloseCamera(string cameraName)
        {
            var isGetCamera = TryGetCamera(cameraName, out var camera);
            if (isGetCamera)
                camera.enabled = false;
        }


        //private method
        private bool TryGetCamera(string cameraName, out Camera camera)
        {
            var isGetValue = _cameras.TryGetValue(cameraName, out camera);
            return isGetValue;
        }
    }
}