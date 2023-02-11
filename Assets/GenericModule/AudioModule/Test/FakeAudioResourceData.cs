using AudioModule;
using UnityEngine;

public class FakeAudioResourceData : IAudioResourceData
{
    //public variable
    public string Key { get; }
    public GameObject Prefab { get; }

    //public method
    public FakeAudioResourceData(string key, GameObject prefab)
    {
        Key = key;
        Prefab = prefab;
    }
}