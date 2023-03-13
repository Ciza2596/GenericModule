using UnityEngine;

namespace CizaAudioModule
{
    public interface IAudioResourceData
    {
        public string Key { get; }
        public GameObject Prefab { get; }
    }
}