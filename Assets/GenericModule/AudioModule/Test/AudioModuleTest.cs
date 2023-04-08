using System.Collections.Generic;
using System.Linq;
using CizaAudioModule;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;


public class AudioModuleTest
{
    private FakeAudioModuleConfig _fakeAudioModuleConfig;
    private FakeAssetProvider _fakeAssetProvider;

    private AudioModule _audioModule;

    private IAudioData[] _audioDatas;
    private Dictionary<string, IAudioData> _audioDataMap;


    [SetUp]
    public void SetUp()
    {
        _fakeAssetProvider = new FakeAssetProvider();
        _fakeAudioModuleConfig = new FakeAudioModuleConfig();

        _audioModule = new AudioModule(_fakeAudioModuleConfig, _fakeAssetProvider);

        _audioDatas = new[]
        {
            new FakeAudioData(FakeAssetProvider.CLIP_DATA_ID, FakeAssetProvider.PREFAB_DATA_ID, FakeAssetProvider.SPATIAL_BLEND),
        };
        _audioDataMap = CreateAudioDataMap(_audioDatas);
    }

    [Test]
    public async void _01_IsInitialized_Is_True_After_AudioModule_Is_Initialized()
    {
        // arrange
        Assert.IsFalse(_audioModule.IsInitialized);
        Assert.IsFalse(_fakeAssetProvider.IsLoadedAssets);
        Assert.IsNull(_fakeAssetProvider.LoadedAssetsDataIds);

        // act
        await _audioModule.Initialize(_audioDataMap);


        // assert
        Assert.IsTrue(_audioModule.IsInitialized, "AudioModule is not initialized.");
        Assert.IsTrue(_fakeAssetProvider.IsLoadedAssets, "Assets is not loaded.");
        Check_String_Array_Content_Is_Equal(FakeAssetProvider.ASSETS_DATA_IDS, _fakeAssetProvider.LoadedAssetsDataIds, "AssetsDataIds is incorrect.");
    }

    [Test]
    public void _02_IsInitialized_Is_False_After_AudioModule_Is_Disposed()
    {
        // arrange
        _01_IsInitialized_Is_True_After_AudioModule_Is_Initialized();


        // act
        _audioModule.Release();


        // assert
        Assert.IsFalse(_audioModule.IsInitialized, "AudioModule is initialized.");
        Assert.IsFalse(_fakeAssetProvider.IsLoadedAssets, "Assets is not unloaded.");
        Check_String_Array_Content_Is_Equal(FakeAssetProvider.ASSETS_DATA_IDS, _fakeAssetProvider.UnloadedAssetsDataIds, "AssetsDataIds is incorrect.");
    }


    public void _03_ReleaseAllPools()
    {
        // //arrange
        // var key1 = "HelloAudio_1";
        // var key2 = "HelloAudio_2";
        // var keys = new List<string>() { key1, key2 }.ToArray();
        // CreateMultFakeAudioResourceDataAndAddToList(keys);
        //
        // var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioData>();
        // Check_AudioModule_Already_Initialize(audioResourceDatas);
        //
        // Check_Pool_Doesnt_Exist_In_Game(key1);
        // Check_Pool_Doesnt_Exist_In_Game(key2);
        //
        // _audioModule.Play(key1);
        // _audioModule.Play(key2);
        //
        // Check_Pool_Exists_In_Game(key1);
        // Check_Pool_Exists_In_Game(key2);
        //
        //
        // //act
        // _audioModule.ReleaseAllPools();
        //
        //
        // //assert
        // Check_Pool_Doesnt_Exist_In_Game(key1);
        // Check_Pool_Doesnt_Exist_In_Game(key2);
    }


    public void _04_ReleasePool_By_Key()
    {
        // //arrange
        // var key1 = "HelloAudio_1";
        // var key2 = "HelloAudio_2";
        // var keys = new List<string>() { key1, key2 }.ToArray();
        // CreateMultFakeAudioResourceDataAndAddToList(keys);
        //
        // var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioData>();
        // Check_AudioModule_Already_Initialize(audioResourceDatas);
        //
        // Check_Pool_Doesnt_Exist_In_Game(key1);
        // Check_Pool_Doesnt_Exist_In_Game(key2);
        //
        // _audioModule.Play(key1);
        // _audioModule.Play(key2);
        //
        // Check_Pool_Exists_In_Game(key1);
        // Check_Pool_Exists_In_Game(key2);
        //
        //
        // //act
        // _audioModule.ReleasePool(key1);
        //
        //
        // //assert
        // Check_Pool_Doesnt_Exist_In_Game(key1);
        // Check_Pool_Exists_In_Game(key2);
    }

    public void _05_GetPoolName_By_Key()
    {
        // //arrange
        // var key = "HelloAudio_1";
        // CreateFakeAudioResourceDataAndAddToList(key);
        //
        // var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioData>();
        // Check_AudioModule_Already_Initialize(audioResourceDatas);
        //
        //
        // //act
        // var poolName = _audioModule.GetPoolName(key);
        //
        //
        // //assert
        // var expectedPoolName = _fakeAudioModuleConfig.PoolPrefix + key + _fakeAudioModuleConfig.PoolSuffix;
        // Assert.AreEqual(expectedPoolName, poolName, "Pool name is not equal.");
    }

    public void _06_CheckIsPlaying_By_Key()
    {
        // //arrange
        // var key = "HelloAudio_1";
        // CreateFakeAudioResourceDataAndAddToList(key);
        //
        // var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioData>();
        // Check_AudioModule_Already_Initialize(audioResourceDatas);
        //
        // var id = _audioModule.Play(key);
        //
        //
        // //act
        // var isPlaying = _audioModule.CheckIsPlaying(id);
        //
        //
        // //assert
        // Assert.IsTrue(isPlaying, "AudioData doesnt play.");
    }

    public void _07_GetAudioData()
    {
        // //arrange
        // var key = "HelloAudio_1";
        // CreateFakeAudioResourceDataAndAddToList(key);
        //
        // var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioData>();
        // Check_AudioModule_Already_Initialize(audioResourceDatas);
        //
        // var id = _audioModule.Play(key);
        //
        //
        // //act
        // var audioData = _audioModule.GetAudio(id);
        //
        //
        // //assert
        // Assert.IsNotNull(audioData, "AudioData is null.");
    }

    public void _08_Play_By_Key_ParentTransform()
    {
        // //arrange
        // var key = "HelloAudio_1";
        // CreateFakeAudioResourceDataAndAddToList(key);
        //
        // var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioData>();
        // Check_AudioModule_Already_Initialize(audioResourceDatas);
        //
        // Check_AudioPlayingTransform_Hasnt_Child();
        //
        //
        // //act
        // var id = _audioModule.Play(key, _audioPlayingTransform);
        //
        //
        // //assert
        // Check_AudioPlayingTransform_Has_Child();
    }

    public void _09_Stop_By_Id()
    {
        // //arrange
        // var key = "HelloAudio_1";
        // CreateFakeAudioResourceDataAndAddToList(key);
        //
        // var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioData>();
        // Check_AudioModule_Already_Initialize(audioResourceDatas);
        //
        // Check_AudioPlayingTransform_Hasnt_Child();
        //
        // var id = _audioModule.Play(key, _audioPlayingTransform);
        // Check_AudioPlayingTransform_Has_Child();
        //
        //
        // //act
        // _audioModule.Stop(id);
        //
        //
        // //assert
        // Check_AudioPlayingTransform_Hasnt_Child();
    }


    // private void Check_AudioModule_Already_Initialize(IAudioData[] audioResourceDatas)
    // {
    //     _audioModule.Initialize(audioResourceDatas);
    //     Assert.IsTrue(_audioModule.IsInitialized);
    // }
    //
    // private void Check_Pool_Doesnt_Exist_In_Game(string key)
    // {
    //     var poolName = _audioModule.GetPoolName(key);
    //     var poolGameObject = GameObject.Find(poolName);
    //     Assert.IsNull(poolGameObject, "Pool already exist.");
    // }
    //
    // private void Check_Pool_Exists_In_Game(string key)
    // {
    //     var poolName = _audioModule.GetPoolName(key);
    //     var poolGameObject = GameObject.Find(poolName);
    //     Assert.IsNotNull(poolGameObject, "Pool doesnt exists.");
    // }
    //
    //
    // private void Check_AudioPlayingTransform_Hasnt_Child() =>
    //     Assert.IsTrue(_audioPlayingTransform.childCount == 0, $"AudioPlayingTransform has child.");
    //
    //
    // private void Check_AudioPlayingTransform_Has_Child() =>
    //     Assert.IsTrue(_audioPlayingTransform.childCount > 0, $"AudioPlayingTransform hasnt child.");
    //
    //
    // private void CreateMultFakeAudioResourceDataAndAddToList(string[] keys)
    // {
    //     foreach (var key in keys)
    //         CreateFakeAudioResourceDataAndAddToList(key);
    // }
    //
    // private void CreateFakeAudioResourceDataAndAddToList(string key)
    // {
    //     var audioPrefab = CreateAudioPrefab(key);
    //     var fakeAudioResourceData = new FakeAudioData(key, audioPrefab);
    //     _fakeAudioResourceDatas.Add(fakeAudioResourceData);
    // }
    //
    // private GameObject CreateAudioPrefab(string key)
    // {
    //     var audioPrefab = new GameObject(key);
    //     audioPrefab.AddComponent<AudioSource>();
    //     return audioPrefab;
    // }

    private Dictionary<string, IAudioData> CreateAudioDataMap(IAudioData[] audioDatas)
    {
        var audioDataMap = new Dictionary<string, IAudioData>();
        foreach (var audioData in audioDatas)
            audioDataMap.Add(audioData.ClipDataId, audioData);

        return audioDataMap;
    }

    private void Check_String_Array_Content_Is_Equal(string[] expectedStrs, string[] strs, string message)
    {
        for (int i = 0; i < expectedStrs.Length; i++)
        {
            var expectedStr = expectedStrs[i];
            var str = strs[i];

            Assert.AreEqual(expectedStr, str, message);
        }
    }
}

public class FakeAudioData : IAudioData
{
    public string ClipDataId { get; }
    public string PrefabDataId { get; }
    public float SpatialBlend { get; }

    public FakeAudioData(string clipDataId, string prefabDataId, float spatialBlend)
    {
        ClipDataId = clipDataId;
        PrefabDataId = prefabDataId;
        SpatialBlend = spatialBlend;
    }
}

public class FakeAssetProvider : IAudioModuleAssetProvider
{
    public const string CLIP_DATA_ID = "ClipDataId";
    public const string PREFAB_DATA_ID = "PrefabDataId";
    public const float SPATIAL_BLEND = 0.5f;

    public static readonly string[] ASSETS_DATA_IDS = new[] { CLIP_DATA_ID, PREFAB_DATA_ID };


    private AudioClip _audioClip;
    private GameObject _audioPrefab;


    public bool IsLoadedAssets { get; private set; }

    public string[] LoadedAssetsDataIds { get; private set; }

    public string[] UnloadedAssetsDataIds { get; private set; }

    public async UniTask LoadAssets<T>(string[] dataIds) where T : Object
    {
        IsLoadedAssets = true;

        if (LoadedAssetsDataIds is null)
            LoadedAssetsDataIds = dataIds;
        
        else
            LoadedAssetsDataIds = LoadedAssetsDataIds.Concat(dataIds).ToArray();

        await UniTask.CompletedTask;
    }

    public void UnloadAssets(string[] dataIds)
    {
        IsLoadedAssets = false;
        UnloadedAssetsDataIds = dataIds;
    }

    public T GetAsset<T>(string dataId) where T : Object
    {
        if (dataId == CLIP_DATA_ID)
            return _audioClip as T;

        if (dataId == PREFAB_DATA_ID)
            return _audioPrefab as T;

        return null;
    }


    public void SetClip(AudioClip audioClip) =>
        _audioClip = audioClip;

    public void SetAudioPrefab(GameObject audioPrefab) =>
        _audioPrefab = audioPrefab;
}