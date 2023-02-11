using System.Collections.Generic;
using System.Linq;
using GameObjectPoolModule;
using NUnit.Framework;
using UnityEngine;


public class GameObjectPoolModuleTest
{
    private string _keyPrefix = "Key";
    private string _prefabNamePrefix = "Prefab";

    private IGameObjectPoolModuleConfig _gameObjectPoolModuleConfig;
    private GameObjectPoolModule.GameObjectPoolModule _gameObjectPoolModule;

    private Transform _spawnTransform;

    [SetUp]
    public void SetUp()
    {
        _gameObjectPoolModuleConfig = new FakeGameObjectPoolModuleConfig();
        _gameObjectPoolModule = new GameObjectPoolModule.GameObjectPoolModule(_gameObjectPoolModuleConfig);

        var spawnGameObject = new GameObject();
        _spawnTransform = spawnGameObject.transform;
    }

    [Test]
    public void _01_Initialize()
    {
        //arrange
        Check_GameObjectPoolModule_Is_Uninitialized();
        var gameObjectResourceDatas = CreateGameObjectResourceDatas(1, _keyPrefix, _prefabNamePrefix);


        //act
        _gameObjectPoolModule.Initialize(gameObjectResourceDatas);


        //assert
        Check_GameObjectPoolModule_Is_Initialized();
    }

    [Test]
    public void _02_Release()
    {
        //arrange
        var gameObjectResourceDatas = CreateGameObjectResourceDatas(1, _keyPrefix, _prefabNamePrefix);
        Check_GameObjectPoolModule_Is_Initialized(gameObjectResourceDatas);


        //act
        _gameObjectPoolModule.Release();


        //assert
        Check_GameObjectPoolModule_Is_Uninitialized();
    }

    [Test]
    public void _03_Spawn_By_Key_ParentTransform()
    {
        //arrange
        var gameObjectResourceDatas = CreateGameObjectResourceDatas(1, _keyPrefix, _prefabNamePrefix);
        Check_GameObjectPoolModule_Is_Initialized(gameObjectResourceDatas);

        Check_SpawnTransform_Hasnt_Children();


        //act
        Spawn_GameObject_To_SpawnTransform(0, gameObjectResourceDatas);


        //assert
        Check_SpawnTransform_Has_Children();
    }

    [Test]
    public void _05_DeSpawn_By_GameObjects()
    {
        //arrange
        var gameObjectResourceDatas = CreateGameObjectResourceDatas(1, _keyPrefix, _prefabNamePrefix);
        Check_GameObjectPoolModule_Is_Initialized(gameObjectResourceDatas);
        
        Check_SpawnTransform_Hasnt_Children();

        var gameObjects = Spawn_GameObjects_To_SpawnTransform(0,2, gameObjectResourceDatas);
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
        var gameObjectResourceDatas = CreateGameObjectResourceDatas(1, _keyPrefix, _prefabNamePrefix);
        Check_GameObjectPoolModule_Is_Initialized(gameObjectResourceDatas);
        
        Check_SpawnTransform_Hasnt_Children();

        var gameObjects = Spawn_GameObjects_To_SpawnTransform(0,1, gameObjectResourceDatas);
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
        var gameObjectResourceDatas = CreateGameObjectResourceDatas(2, _keyPrefix, _prefabNamePrefix);
        Check_GameObjectPoolModule_Is_Initialized(gameObjectResourceDatas);
        
        Check_SpawnTransform_Hasnt_Children();

        Spawn_GameObject_To_SpawnTransform(0, gameObjectResourceDatas);
        Spawn_GameObject_To_SpawnTransform(1, gameObjectResourceDatas);
        Check_SpawnTransform_Has_Children(2);

        
        //act
        var key = GetKey(0, gameObjectResourceDatas);
        _gameObjectPoolModule.DeSpawn(key);


        //assert
        Check_SpawnTransform_Has_Children();
    }

    [Test]
    public void _08_DeSpawnAll()
    {
        //arrange
        var gameObjectResourceDatas = CreateGameObjectResourceDatas(2, _keyPrefix, _prefabNamePrefix);
        Check_GameObjectPoolModule_Is_Initialized(gameObjectResourceDatas);
        
        Check_SpawnTransform_Hasnt_Children();

        Spawn_GameObject_To_SpawnTransform(0, gameObjectResourceDatas);
        Spawn_GameObject_To_SpawnTransform(1, gameObjectResourceDatas);
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
        var gameObjectResourceDatas = CreateGameObjectResourceDatas(1, _keyPrefix, _prefabNamePrefix);
        Check_GameObjectPoolModule_Is_Initialized(gameObjectResourceDatas);
        
        var key = GetKey(0, gameObjectResourceDatas);
        Check_Not_Exist_Pool(key);
        
        Spawn_GameObject_To_SpawnTransform(0, gameObjectResourceDatas);
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
        var gameObjectResourceDatas = CreateGameObjectResourceDatas(2, _keyPrefix, _prefabNamePrefix);
        Check_GameObjectPoolModule_Is_Initialized(gameObjectResourceDatas);
        
        var key1 = GetKey(0, gameObjectResourceDatas);
        var key2 = GetKey(1, gameObjectResourceDatas);

        Spawn_GameObject_To_SpawnTransform(0, gameObjectResourceDatas);
        Spawn_GameObject_To_SpawnTransform(1, gameObjectResourceDatas);

        Check_Exist_Pool(key1);
        Check_Exist_Pool(key2);
        
        
        //act
        _gameObjectPoolModule.ReleaseAllPool();
        
        
        //assert
        Check_Not_Exist_Pool(key1);
        Check_Not_Exist_Pool(key2);
        
    }


    //private method
    private IGameObjectResourceData[] CreateGameObjectResourceDatas(int length, string keyPrefix,
        string prefabNamePrefix)
    {
        Assert.IsTrue(length > 0, "Length less one.");

        var gameObjectResourceDatas = new List<FakeGameObjectResourceData>();

        for (int i = 0; i < length; i++)
        {
            var key = $"{keyPrefix}_{i}";
            var prefabName = $"{prefabNamePrefix}_{i}";
            var prefab = new GameObject(prefabName);

            var fakeGameObjectResourceData = new FakeGameObjectResourceData(key, prefab);
            gameObjectResourceDatas.Add(fakeGameObjectResourceData);
        }

        return gameObjectResourceDatas.ToArray<IGameObjectResourceData>();
    }

    private void Check_GameObjectPoolModule_Is_Initialized(IGameObjectResourceData[] gameObjectResourceDatas)
    {
        _gameObjectPoolModule.Initialize(gameObjectResourceDatas);
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
        IGameObjectResourceData[] gameObjectResourceDatas)
    {
        var gameObjectResourceData = gameObjectResourceDatas[keyIndex];
        var key = gameObjectResourceData.Key;
        return _gameObjectPoolModule.Spawn(key, _spawnTransform);
    }

    private GameObject[] Spawn_GameObjects_To_SpawnTransform(int keyIndex, int count,
        IGameObjectResourceData[] gameObjectResourceDatas)
    {
        var gameObjects = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            var gameObject = Spawn_GameObject_To_SpawnTransform(keyIndex, gameObjectResourceDatas);
            gameObjects.Add(gameObject);
        }

        return gameObjects.ToArray();
    }

    private string GetKey(int keyIndex, IGameObjectResourceData[] gameObjectResourceDatas)
    {
        var gameObjectResourceData = gameObjectResourceDatas[keyIndex];
        return gameObjectResourceData.Key;
    }
}