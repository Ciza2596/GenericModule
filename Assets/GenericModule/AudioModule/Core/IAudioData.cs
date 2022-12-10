using UnityEngine;

namespace AudioModule
{
    public interface IAudioData
    {
        public string Key { get; }
        public GameObject Prefab { get; }
    }
}