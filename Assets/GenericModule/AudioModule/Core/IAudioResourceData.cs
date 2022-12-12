using UnityEngine;

namespace AudioModule
{
    public interface IAudioResourceData
    {
        public string Key { get; }
        public GameObject Prefab { get; }
    }
}