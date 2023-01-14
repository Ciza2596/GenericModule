using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;



namespace GameObjectPoolModule
{
    public class GameObjectPoolModule
    {
        //private variable
        private readonly Transform _poolRootTransform;
        private readonly string _poolPrefix;
        private readonly string _poolSuffix;
        private readonly Dictionary<string, Transform> _poolTransforms = new Dictionary<string, Transform>();
        
        
        private readonly Dictionary<string, List<GameObject>> _pools = new Dictionary<string, List<GameObject>>();
        private readonly Dictionary<GameObject, string> _gameObjectKeyMaps = new Dictionary<GameObject, string>();
        private readonly List<GameObject> _usingGameObjects = new List<GameObject>();


        private List<IGameObjectResourceData> _gameObjectResourceDatas;

        

        //public method
        public GameObjectPoolModule(IGameObjectPoolModuleConfig gameObjectPoolModuleConfig)
        {
            var poolRootGameObject = new GameObject(gameObjectPoolModuleConfig.PoolRootName);
            
            _poolRootTransform = poolRootGameObject.transform;
            _poolRootTransform.SetParent(gameObjectPoolModuleConfig.PoolRootTransform);

            _poolPrefix = gameObjectPoolModuleConfig.PoolPrefix;
            _poolSuffix = gameObjectPoolModuleConfig.PoolSuffix;
        }


        public void SetGameObjectResourceDatas(List<IGameObjectResourceData> gameObjectResourceDatas)
        {
            ReleaseAll();
            _gameObjectResourceDatas = gameObjectResourceDatas;
        }


        public GameObject Spawn(string key)
        {
            if (!_pools.ContainsKey(key))
                CreatePool(key);

            var pool = _pools[key];

            GameObject gameObject = null;
            
            if(pool.Count == 0)
                gameObject = CreateGameObject(key);
            
            else
            {
                gameObject = pool[0];
                pool.Remove(gameObject);
            }
            
            _usingGameObjects.Add(gameObject);

            return gameObject;
        }

        public void DeSpawn(GameObject gameObject)
        {
            Assert.IsTrue(!_usingGameObjects.Contains(gameObject), "[PoolModule::DeSpawn] Not find gameObject from usingGameObjects.");

            _usingGameObjects.Remove(gameObject);

            var key = _gameObjectKeyMaps[gameObject];
            var poolTransform = _poolTransforms[key];

            var transform = gameObject.transform;
            transform.SetParent(poolTransform);

            var gameObjects = _pools[key];
            gameObjects.Add(gameObject);
        }

        public void Release(string key)
        {
            Assert.IsTrue(_pools.ContainsKey(key),$"[PoolModule::Release] Not find pool. Please check key:{key}");
            
            var keyValuePairs = _gameObjectKeyMaps.Where(pair => pair.Value == key).ToArray();
            Assert.IsNotNull(keyValuePairs,$"[PoolModule::Release] Not find gameObject from _gameObjectKeyMaps. Please check key: {key}");

            foreach (var keyValuePair in keyValuePairs)
            {
                var gameObject = keyValuePair.Key;
                
                DeSpawn(gameObject);
                _gameObjectKeyMaps.Remove(gameObject);
            }


            var pool  = _pools[key];
            pool.Clear();
            _pools.Remove(key);

            var gameObjects = pool.ToArray();
            foreach (var gameObject in gameObjects)
                Object.Destroy(gameObject);


            var poolTransform = _poolTransforms[key];
            Object.Destroy(poolTransform.gameObject);
        }

        public void ReleaseAll()
        {
            var keys = _pools.Keys.ToArray();
            foreach (var key in keys)
                Release(key);
        }
        
        
        //private method
        private void CreatePool(string key)
        {
            var gameObjects = new List<GameObject>();
            _pools.Add(key, gameObjects);

            var gameObject = new GameObject( _poolPrefix + key + _poolSuffix);
            var poolTransform = gameObject.transform;
            _poolTransforms.Add(key, poolTransform);
        }

        private GameObject CreateGameObject(string key)
        {
            var gameObjectResourceData = _gameObjectResourceDatas.Find(data => data.Key == key);
            Assert.IsNotNull(gameObjectResourceData,$"[PoolModule::CreateGameObject] Not find gameObjectResourceData. Please check key: {key}.");
            
            var prefab = gameObjectResourceData.Prefab;
            Assert.IsNotNull(prefab,$"[PoolModule::CreateGameObject] Not find prefab. Please check key: {key}.");
            
            var gameObject = Object.Instantiate(prefab);
            _gameObjectKeyMaps.Add(gameObject, key);
            
            return gameObject;
        }
    }
}
