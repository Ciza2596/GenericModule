

using UnityEngine;

namespace CizaGameObjectPoolModule
{
    public interface IGameObjectResourceData
    {
        public string Key { get; }
        public GameObject Prefab { get; }
    }
}

