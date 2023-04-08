using CizaAudioModule;
using UnityEngine;

public class FakeAudioData : IAudioData
{
    //public variable
    public string ClipDataId { get; }
    public string PrefabDataId { get; }
    public float SpatialBlend { get; }
    public GameObject Prefab { get; }

    //public method
    public FakeAudioData(string key, GameObject prefab)
    {
        ClipDataId = key;
        Prefab = prefab;
    }
}