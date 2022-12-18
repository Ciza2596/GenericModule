

using UnityEngine;

namespace GameObjectPoolModule
{
    public interface IGameObjectResourceData
    {
        public string Key { get; }
        public GameObject Prefab { get; }
    }
}

