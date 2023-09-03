using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaGameObjectPoolModule
{
	public class GameObjectPoolModule
	{
		//private variable

		private readonly IGameObjectPoolModuleConfig _gameObjectPoolModuleConfig;
		private          Transform                   _poolRootTransform;

		private readonly Dictionary<string, Transform> _poolTransforms = new Dictionary<string, Transform>();

		private readonly Dictionary<string, List<GameObject>> _pools             = new Dictionary<string, List<GameObject>>();
		private readonly Dictionary<GameObject, string>       _gameObjectKeyMaps = new Dictionary<GameObject, string>();
		private readonly List<GameObject>                     _usingGameObjects  = new List<GameObject>();

		private Dictionary<string, GameObject> _prefabMap;

		//public variable
		public bool IsInitialized => _prefabMap != null && _poolRootTransform != null;

		//constructor
		public GameObjectPoolModule(IGameObjectPoolModuleConfig gameObjectPoolModuleConfig) =>
			_gameObjectPoolModuleConfig = gameObjectPoolModuleConfig;

		//public method
		public void Initialize(Dictionary<string, GameObject> prefabMap)
		{
			Assert.IsNotNull(prefabMap, "[GameObjectPoolModule::Initialize] PrefabMap is null.");

			_prefabMap = prefabMap;

			if (_poolRootTransform is null)
			{
				var poolRootGameObject = new GameObject(_gameObjectPoolModuleConfig.PoolRootName);
				_poolRootTransform = poolRootGameObject.transform;
			}
		}

		public void Release()
		{
			_prefabMap = null;
			ReleaseAllPool();

			var poolRootGameObject = _poolRootTransform.gameObject;
			_poolRootTransform = null;
			DestroyOrImmediate(poolRootGameObject);
		}

		public bool TryGetPoolRootName(out string poolRootName)
		{
			poolRootName = _gameObjectPoolModuleConfig.PoolRootName;
			return _poolRootTransform != null;
		}

		public bool TryGetPoolName(string key, out string poolName)
		{
			poolName = GetPoolName(key);

			var hasPool = _poolTransforms.ContainsKey(key);
			return hasPool;
		}

		public GameObject Spawn(string key, Vector3 position = default, Transform parentTransform = null, bool isLocalPosition = false)
		{
			if (!_prefabMap.ContainsKey(key))
			{
				Debug.LogWarning($"[GameObjectPoolModule::Spawn] Not find prefab by key: {key}.");
				return null;
			}

			if (!_pools.ContainsKey(key))
				CreatePool(key);

			var pool = _pools[key];

			GameObject gameObject = null;

			if (pool.Count == 0)
				gameObject = CreateGameObject(key);

			else
			{
				gameObject = pool[0];
				pool.Remove(gameObject);
			}

			_usingGameObjects.Add(gameObject);

			var transform = gameObject.transform;
			transform.SetParent(parentTransform);

			if (isLocalPosition)
				transform.localPosition = position;
			else
				transform.position = position;

			gameObject.SetActive(true);
			return gameObject;
		}

		public void DeSpawn(GameObject[] gameObjects)
		{
			foreach (var gameObject in gameObjects)
				DeSpawn(gameObject);
		}

		public void DeSpawn(GameObject gameObject)
		{
			Assert.IsTrue(_usingGameObjects.Contains(gameObject),
			              "[PoolModule::DeSpawn] Not find gameObject from usingGameObjects.");

			gameObject.SetActive(false);
			_usingGameObjects.Remove(gameObject);

			var key           = _gameObjectKeyMaps[gameObject];
			var poolTransform = _poolTransforms[key];

			var transform = gameObject.transform;
			transform.SetParent(poolTransform);

			var gameObjects = _pools[key];
			gameObjects.Add(gameObject);
		}

		public void DeSpawn(string key)
		{
			var usingGameObjects = GetUsingGameObjects(key);
			DeSpawn(usingGameObjects);
		}

		public void DeSpawnAll()
		{
			var usingGameObjects = _usingGameObjects.ToArray();
			DeSpawn(usingGameObjects);
		}

		public void ReleasePool(string key)
		{
			Assert.IsTrue(_pools.ContainsKey(key), $"[PoolModule::Release] Not find pool. Please check key:{key}");

			var keyValuePairs = _gameObjectKeyMaps.Where(pair => pair.Value == key).ToArray();
			Assert.IsNotNull(keyValuePairs,
			                 $"[PoolModule::Release] Not find gameObject from _gameObjectKeyMaps. Please check key: {key}");

			foreach (var keyValuePair in keyValuePairs)
			{
				var gameObject = keyValuePair.Key;

				if (_usingGameObjects.Contains(gameObject))
					DeSpawn(gameObject);
				_gameObjectKeyMaps.Remove(gameObject);
			}


			var pool = _pools[key];
			pool.Clear();
			_pools.Remove(key);

			var gameObjects = pool.ToArray();
			foreach (var gameObject in gameObjects)
				Object.Destroy(gameObject);


			var poolTransform = _poolTransforms[key];
			_poolTransforms.Remove(key);
			DestroyOrImmediate(poolTransform.gameObject);
		}

		public void ReleaseAllPool()
		{
			var keys = _pools.Keys.ToArray();
			foreach (var key in keys)
				ReleasePool(key);
		}

		//private method
		private string GetPoolName(string key) => _gameObjectPoolModuleConfig.PoolPrefix + key + _gameObjectPoolModuleConfig.PoolSuffix;

		private void CreatePool(string key)
		{
			var gameObjects = new List<GameObject>();
			_pools.Add(key, gameObjects);

			var poolName      = GetPoolName(key);
			var gameObject    = new GameObject(poolName);
			var poolTransform = gameObject.transform;
			poolTransform.SetParent(_poolRootTransform);
			_poolTransforms.Add(key, poolTransform);
		}

		private GameObject CreateGameObject(string key)
		{
			var prefab     = _prefabMap[key];
			var gameObject = Object.Instantiate(prefab);
			_gameObjectKeyMaps.Add(gameObject, key);

			return gameObject;
		}

		private GameObject[] GetUsingGameObjects(string key)
		{
			var usingGameObjects      = _usingGameObjects.ToArray();
			var usingGameObjectsByKey = new List<GameObject>();

			foreach (var usingGameObject in usingGameObjects)
			{
				var usingKey = _gameObjectKeyMaps[usingGameObject];
				if (usingKey == key)
					usingGameObjectsByKey.Add(usingGameObject);
			}

			return usingGameObjectsByKey.ToArray();
		}

		private void DestroyOrImmediate(Object obj)
		{
			if (Application.isPlaying)
				Object.Destroy(obj);
			else
				Object.DestroyImmediate(obj);
		}
	}
}
