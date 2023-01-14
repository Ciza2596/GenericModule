using UnityEngine;

namespace GameObjectPoolModule.Implement
{
    [CreateAssetMenu(fileName = "GameObjectPoolModuleConfig", menuName = "GameObjectPoolModule/GameObjectPoolModuleConfig")]
    public class GameObjectPoolModuleConfig : ScriptableObject, IGameObjectPoolModuleConfig
    {
        [SerializeField]
        private string _poolRootName = "[GameObjectModule]";
        
        [Space]
        [SerializeField] private string _poolPrefix = "[";
        [SerializeField] private string _poolSuffix = "s]";

        public string PoolRootName => _poolRootName;
        public string PoolPrefix => _poolPrefix;
        public string PoolSuffix => _poolSuffix;
    }
}