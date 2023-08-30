using System.Collections.Generic;
using System.Linq;
using CizaGameObjectPoolModule;
using NUnit.Framework;
using UnityEngine;


public class GameObjectPoolModuleTest
{
    private string _keyPrefix = "Key";
    private string _prefabNamePrefix = "Prefab";

    private IGameObjectPoolModuleConfig _gameObjectPoolModuleConfig;
    private CizaGameObjectPoolModule.GameObjectPoolModule _gameObjectPoolModule;

    private Transform _spawnTransform;

    [SetUp]
    public void SetUp()
    {
        _gameObjectPoolModuleConfig = new FakeGameObjectPoolModuleConfig();
        _gameObjectPoolModule = new CizaGameObjectPoolModule.GameObjectPoolModule(_gameObjectPoolModuleConfig);

        var spawnGameObject = new GameObject();
        _spawnTransform = spawnGameObject.transform;
    }

    [Test]
    public void _01_Initialize()
    {
        //arrange
        Check_GameObjectPoolModule_Is_Uninitialized();
        var prefabMap = CreatePrefabMap(1, _keyPrefix, _prefabNamePrefix);


        //act
        _gameObjectPoolModule.Initialize(prefabMap);


        //assert
        Check_GameObjectPoolModule_Is_Initialized();
    }

    [Test]
    public void _02_Release()
    {
        //arrange
        var prefabMap = CreatePrefabMap(1, _keyPrefix, _prefabNamePrefix);
        Check_GameObjectPoolModule_Is_Initialized(prefabMap);


        //act
        _gameObjectPoolModule.Release();


        //assert
        Check_GameObjectPoolModule_Is_Uninitialized();
    }

    [Test]
    public void _03_Spawn_By_Key_ParentTransform()
    {
        //arrange
        var prefabMap = CreatePrefabMap(1, _keyPrefix, _prefabNamePrefix);
        Check_GameObjectPoolModule_Is_Initialized(prefabMap);

        Check_SpawnTransform_Hasnt_Children();


        //act
        Spawn_GameObject_To_SpawnTransform(0, prefabMap);


        //assert
        Check_SpawnTransform_Has_Children();
    }

    [Test]
    public void _05_DeSpawn_By_GameObjects()
    {
        //arrange
        var prefabMap = CreatePrefabMap(1, _keyPrefix, _prefabNamePrefix);
        Check_GameObjectPoolModule_Is_Initialized(prefabMap);

        Check_SpawnTransform_Hasnt_Children();

        var gameObjects = Spawn_GameObjects_To_SpawnTransform(0, 2, prefabMap);
        Check_SpawnTransform_Has_Children(2);


        //act
        _gameObjectPoolModule.DeSpawn(gameObjects);


        //assert
        Check_SpawnTransform_Hasnt_Children();
    }

    [Test]
    public void _06_DeSpawn_By_GameObject()
    {
        //arrange
        var prefabMap = CreatePrefabMap(1, _keyPrefix, _prefabNamePrefix);
        Check_GameObjectPoolModule_Is_Initialized(prefabMap);

        Check_SpawnTransform_Hasnt_Children();

        var gameObjects = Spawn_GameObjects_To_SpawnTransform(0, 1, prefabMap);
        Check_SpawnTransform_Has_Children();


        //act
        var gameObject = gameObjects[0];
        _gameObjectPoolModule.DeSpawn(gameObject);


        //assert
        Check_SpawnTransform_Hasnt_Children();
    }

    [Test]
    public void _07_DeSpawn_By_Key()
    {
        //arrange
        var prefabMap = CreatePrefabMap(2, _keyPrefix, _prefabNamePrefix);
        Check_GameObjectPoolModule_Is_Initialized(prefabMap);

        Check_SpawnTransform_Hasnt_Children();

        Spawn_GameObject_To_SpawnTransform(0, prefabMap);
        Spawn_GameObject_To_SpawnTransform(1, prefabMap);
        Check_SpawnTransform_Has_Children(2);


        //act
        var key = GetKey(0, prefabMap);
        _gameObjectPoolModule.DeSpawn(key);


        //assert
        Check_SpawnTransform_Has_Children();
    }

    [Test]
    public void _08_DeSpawnAll()
    {
        //arrange
        var prefabMap = CreatePrefabMap(2, _keyPrefix, _prefabNamePrefix);
        Check_GameObjectPoolModule_Is_Initialized(prefabMap);

        Check_SpawnTransform_Hasnt_Children();

        Spawn_GameObject_To_SpawnTransform(0, prefabMap);
        Spawn_GameObject_To_SpawnTransform(1, prefabMap);
        Check_SpawnTransform_Has_Children(2);


        //act
        _gameObjectPoolModule.DeSpawnAll();


        //assert
        Check_SpawnTransform_Hasnt_Children();
    }

    [Test]
    public void _09_ReleasePool()
    {
        //arrange
        var prefabMap = CreatePrefabMap(1, _keyPrefix, _prefabNamePrefix);
        Check_GameObjectPoolModule_Is_Initialized(prefabMap);

        var key = GetKey(0, prefabMap);
        Check_Not_Exist_Pool(key);

        Spawn_GameObject_To_SpawnTransform(0, prefabMap);
        Check_Exist_Pool(key);


        //act
        _gameObjectPoolModule.ReleasePool(key);


        //assert
        Check_Not_Exist_Pool(key);
    }

    [Test]
    public void _10_ReleaseAllPool()
    {
        //arrange
        var prefabMap = CreatePrefabMap(2, _keyPrefix, _prefabNamePrefix);
        Check_GameObjectPoolModule_Is_Initialized(prefabMap);

        var key1 = GetKey(0, prefabMap);
        var key2 = GetKey(1, prefabMap);

        Spawn_GameObject_To_SpawnTransform(0, prefabMap);
        Spawn_GameObject_To_SpawnTransform(1, prefabMap);

        Check_Exist_Pool(key1);
        Check_Exist_Pool(key2);


        //act
        _gameObjectPoolModule.ReleaseAllPool();


        //assert
        Check_Not_Exist_Pool(key1);
        Check_Not_Exist_Pool(key2);
    }


    //private method
    private Dictionary<string, GameObject> CreatePrefabMap(int length, string keyPrefix,
        string prefabNamePrefix)
    {
        Assert.IsTrue(length > 0, "Length less one.");

        var prefabMap = new Dictionary<string, GameObject>();

        for (int i = 0; i < length; i++)
        {
            var key = $"{keyPrefix}_{i}";
            var prefabName = $"{prefabNamePrefix}_{i}";
            var prefab = new GameObject(prefabName);
            prefabMap.Add(key, prefab);
        }

        return prefabMap;
    }

    private void Check_GameObjectPoolModule_Is_Initialized(Dictionary<string, GameObject> prefabMap)
    {
        _gameObjectPoolModule.Initialize(prefabMap);
        Check_GameObjectPoolModule_Is_Initialized();
    }

    private void Check_GameObjectPoolModule_Is_Initialized() => Assert.IsTrue(_gameObjectPoolModule.IsInitialized,
        "GameObjectPoolModule is Uninitialized.");

    private void Check_GameObjectPoolModule_Is_Uninitialized() =>
        Assert.IsFalse(_gameObjectPoolModule.IsInitialized,
            "GameObjectPoolModule is initialized.");


    private void Check_SpawnTransform_Has_Children(int count = 1) =>
        Assert.IsTrue(_spawnTransform.childCount >= count, $"SpawnTransform hasnt {count} children.");

    private void Check_SpawnTransform_Hasnt_Children(int count = 0) =>
        Assert.IsTrue(_spawnTransform.childCount <= 0, $"SpawnTransform has children.");

    private void Check_Exist_Pool(string key)
    {
        var hasPool = _gameObjectPoolModule.TryGetPoolName(key, out var poolName);
        Assert.IsTrue(hasPool, $"GameObjectPoolModule doesnt spawn {key} pool.");
    }

    private void Check_Not_Exist_Pool(string key)
    {
        var hasPool = _gameObjectPoolModule.TryGetPoolName(key, out var poolName);
        Assert.IsFalse(hasPool, $"GameObjectPoolModule spawns {key} pool.");
    }

    private GameObject Spawn_GameObject_To_SpawnTransform(int keyIndex,
        Dictionary<string, GameObject> prefabMap)
    {
        var key = prefabMap.Keys.ToArray()[keyIndex];
        return _gameObjectPoolModule.Spawn(key, parentTransform: _spawnTransform);
    }

    private GameObject[] Spawn_GameObjects_To_SpawnTransform(int keyIndex, int count,
        Dictionary<string, GameObject> prefabMap)
    {
        var gameObjects = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            var gameObject = Spawn_GameObject_To_SpawnTransform(keyIndex, prefabMap);
            gameObjects.Add(gameObject);
        }

        return gameObjects.ToArray();
    }

    private string GetKey(int keyIndex, Dictionary<string, GameObject> prefabMap)
    {
        var key = prefabMap.Keys.ToArray()[keyIndex];
        return key;
    }
}