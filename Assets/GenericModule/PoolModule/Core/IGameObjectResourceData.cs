

using UnityEngine;

namespace PoolModule
{
    public interface IGameObjectResourceData
    {
        public string Key { get; }
        public GameObject Prefab { get; }
    }
}

