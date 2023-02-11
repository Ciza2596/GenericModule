using GameObjectPoolModule;
using UnityEngine;

public class FakeGameObjectResourceData : IGameObjectResourceData
{
    //public variable
    public string Key { get; }
    public GameObject Prefab { get; }

    //constructor
    public FakeGameObjectResourceData(string key, GameObject prefab)
    {
        Key = key;
        Prefab = prefab;
    }
}