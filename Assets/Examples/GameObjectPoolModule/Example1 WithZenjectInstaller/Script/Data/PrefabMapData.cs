using System;
using UnityEngine;

namespace CizaGameObjectPoolModule.Example1
{
    [Serializable]
    public class PrefabMapData
    {

        [SerializeField] private string _key;
        [SerializeField] private GameObject _prefab;

        public string Key => _key;
        public GameObject Prefab => _prefab;
    }
}