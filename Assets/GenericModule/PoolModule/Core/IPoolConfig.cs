using UnityEngine;


namespace PoolModule
{
    public interface IPoolConfig
    {
        public string PoolRootName { get; }
        public Transform PoolRootTransform { get; }
        
        public string Prefix { get; }
    }
}
