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

        Check_SpawnTransform_Hasnt_Child();
        
        var gameObjectResourceData = gameObjectResourceDatas[0];
        var key = gameObjectResourceData.Key;

        
        //act
        _gameObjectPoolModule.Spawn(key, _spawnTransform);
        
        
        //assert
        Check_SpawnTransform_Has_Child();
    }

    [Test]
    public void _05_DeSpawn_By_GameObjects()
    {
    }

    [Test]
    public void _06_DeSpawn_By_GameObject()
    {
    }
    
    [Test]
    public void _07_DeSpawnAll_By_Key()
    {
    }
    
    [Test]
    public void _08_DeSpawnAll()
    {
    }

    [Test]
    public void _09_ReleasePool()
    {
    }

    [Test]
    public void _10_ReleaseAllPool()
    {
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


    private void Check_SpawnTransform_Has_Child() =>
        Assert.IsTrue(_spawnTransform.childCount > 0, $"SpawnTransform hasnt child.");
    
    private void Check_SpawnTransform_Hasnt_Child() =>
        Assert.IsTrue(_spawnTransform.childCount == 0, $"SpawnTransform has child.");
}

public class FakeGameObjectPoolModuleConfig : IGameObjectPoolModuleConfig
{
    //public variable
    public string PoolRootName { get; } = "[GameObjectModule]";
    public string PoolPrefix { get; } = "[";
    public string PoolSuffix { get; } = "s]";
}

public class FakeGameObjectResourceData : IGameObjectResourceData
{
    //public variable
    public string Key { get; }
    public GameObject Prefab { get; }

    //constructor
    public FakeGameObjectResourceData(string key, GameObject prefab)
    {
        Key = key;
        Prefab = prefab;
    }
}