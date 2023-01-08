using UnityEngine;


namespace GameObjectPoolModule
{
    public interface IGameObjectPoolModuleConfig
    {
        public string PoolRootName { get; }
        public Transform PoolRootTransform { get; }
        
        public string Prefix { get; }
    }
}
