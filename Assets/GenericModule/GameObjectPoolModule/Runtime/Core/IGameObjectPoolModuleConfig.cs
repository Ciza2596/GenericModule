using UnityEngine;


namespace GameObjectPoolModule
{
    public interface IGameObjectPoolModuleConfig
    {
        public string PoolRootName { get; }
        public string PoolPrefix { get; }
        public string PoolSuffix { get; }
    }
}
