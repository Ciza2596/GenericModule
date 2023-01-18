using System.Collections.Generic;
using System.Linq;
using AudioModule;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;


public class AudioModuleTest
{
    private FakeAudioModuleConfig _fakeAudioModuleConfig;
    private AudioModule.AudioModule _audioModule;

    private List<FakeAudioResourceData> _fakeAudioResourceDatas;

    [SetUp]
    public void SetUp()
    {
        _fakeAudioModuleConfig = new FakeAudioModuleConfig();
        _audioModule = new AudioModule.AudioModule(_fakeAudioModuleConfig);

        _fakeAudioResourceDatas = new List<FakeAudioResourceData>();
    }


    [Test]
    public void _01_Should_Be_Initialized_When_Initialize()
    {
        //arrange
        Assert.IsFalse(_audioModule.IsInitialized);
        var fakeAudioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioResourceData>();

        
        //act
        _audioModule.Initialize(fakeAudioResourceDatas);

        
        //assert
        Assert.IsTrue(_audioModule.IsInitialized, "AudioModule initialize success.");
    }

    [Test]
    public void _02_Should_Not_Be_Initialized_When_Release()
    {
        //arrange
        Assert.IsFalse(_audioModule.IsInitialized);

        var fakeAudioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioResourceData>();
        _audioModule.Initialize(fakeAudioResourceDatas);
        Assert.IsTrue(_audioModule.IsInitialized);

        
        //act
        _audioModule.Release();

        
        //assert
        Assert.IsFalse(_audioModule.IsInitialized, "AudioModule release failed.");
    }

    [Test]
    public void _03_Should_ReleaseAllPools_When_ReleaseAllPools()
    {
        //arrange
        var key1 = "HelloAudio_1";
        var key2 = "HelloAudio_2";
        var keys = new List<string>() { key1, key2 }.ToArray();
        CreateMultFakeAudioResourceDataAndAddToList(keys);

        var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioResourceData>();
        Check_AudioModule_Already_Initialize(audioResourceDatas);

        Check_Pool_Doesnt_Exist_In_Game(key1);
        Check_Pool_Doesnt_Exist_In_Game(key2);

        _audioModule.Play(key1);
        _audioModule.Play(key2);

        Check_Pool_Exists_In_Game(key1);
        Check_Pool_Exists_In_Game(key2);


        //act
        _audioModule.ReleaseAllPools();


        //assert
        Check_Pool_Doesnt_Exist_In_Game(key1);
        Check_Pool_Doesnt_Exist_In_Game(key2);
    }


    [Test]
    public void _04_Should_ReleasePool_When_ReleasePool()
    {
        //arrange
        var key1 = "HelloAudio_1";
        var key2 = "HelloAudio_2";
        var keys = new List<string>() { key1, key2 }.ToArray();
        CreateMultFakeAudioResourceDataAndAddToList(keys);

        var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioResourceData>();
        Check_AudioModule_Already_Initialize(audioResourceDatas);

        Check_Pool_Doesnt_Exist_In_Game(key1);
        Check_Pool_Doesnt_Exist_In_Game(key2);

        _audioModule.Play(key1);
        _audioModule.Play(key2);

        Check_Pool_Exists_In_Game(key1);
        Check_Pool_Exists_In_Game(key2);


        //act
        _audioModule.ReleasePool(key1);


        //assert
        Check_Pool_Doesnt_Exist_In_Game(key1);
        Check_Pool_Exists_In_Game(key2);
    }

    [Test]
    public void _05_Should_GetPoolName_When_GetPoolName()
    {
        //arrange
        var key = "HelloAudio_1";
        CreateFakeAudioResourceDataAndAddToList(key);

        var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioResourceData>();
        Check_AudioModule_Already_Initialize(audioResourceDatas);

        
        //act
        var poolName = _audioModule.GetPoolName(key);

        
        //assert
        var expectedPoolName = _fakeAudioModuleConfig.PoolPrefix + key + _fakeAudioModuleConfig.PoolSuffix;
        Assert.AreEqual(expectedPoolName, poolName, "Pool name is not equal.");
    }

    [Test]
    public void _06_CheckIsPlaying()
    {
        //arrange
        var key = "HelloAudio_1";
        CreateFakeAudioResourceDataAndAddToList(key);

        var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioResourceData>();
        Check_AudioModule_Already_Initialize(audioResourceDatas);

        var id = _audioModule.Play(key);

        
        //act
        var isPlaying = _audioModule.CheckIsPlaying(id);
        
        
        //assert
        Assert.IsTrue(isPlaying, "AudioData doesnt play.");
    }

    [Test]
    public void _07_GetAudioData()
    {
        //arrange
        var key = "HelloAudio_1";
        CreateFakeAudioResourceDataAndAddToList(key);

        var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioResourceData>();
        Check_AudioModule_Already_Initialize(audioResourceDatas);

        var id = _audioModule.Play(key);

        
        //act
        var audioData = _audioModule.GetAudioData(id);
        
        
        //assert
        Assert.IsNotNull(audioData, "AudioData is null.");
    }


    [Test]
    public void _08_Play()
    {
        //arrange
        var key = "HelloAudio_1";
        CreateFakeAudioResourceDataAndAddToList(key);

        var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioResourceData>();
        Check_AudioModule_Already_Initialize(audioResourceDatas);

        var audioPlayingGameObject= new GameObject();
        var audioPlayingTransform = audioPlayingGameObject.transform;
        Check_Transform_Hasnt_Child(audioPlayingTransform);

        
        //act
        var id = _audioModule.Play(key, audioPlayingTransform);
        
        
        //assert
        Check_Transform_Has_Child(audioPlayingTransform);
    }

    public void Stop()
    {
        //arrange
        var key = "HelloAudio_1";
        CreateFakeAudioResourceDataAndAddToList(key);

        var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioResourceData>();
        Check_AudioModule_Already_Initialize(audioResourceDatas);

        var audioPlayingGameObject= new GameObject();
        var audioPlayingTransform = audioPlayingGameObject.transform;
        Check_Transform_Hasnt_Child(audioPlayingTransform);
        
        var id = _audioModule.Play(key, audioPlayingTransform);
        Check_Transform_Has_Child(audioPlayingTransform);

        
        //act
        _audioModule.Stop(key);
        
        
        //assert
        Check_Transform_Hasnt_Child(audioPlayingTransform);
    }


    private void Check_AudioModule_Already_Initialize(IAudioResourceData[] audioResourceDatas)
    {
        _audioModule.Initialize(audioResourceDatas);
        Assert.IsTrue(_audioModule.IsInitialized);
    }

    private void Check_Pool_Doesnt_Exist_In_Game(string key)
    {
        var poolName = _audioModule.GetPoolName(key);
        var poolGameObject = GameObject.Find(poolName);
        Assert.IsNull(poolGameObject, "Pool already exist.");
    }
    private void Check_Pool_Exists_In_Game(string key)
    {
        var poolName = _audioModule.GetPoolName(key);
        var poolGameObject = GameObject.Find(poolName);
        Assert.IsNotNull(poolGameObject, "Pool doesnt exists.");
    }

    
    private void Check_Transform_Hasnt_Child(Transform transform) =>
        Assert.IsTrue(transform.childCount == 0, $"Transform: {transform.name} has child.");
    private void Check_Transform_Has_Child(Transform transform) =>
        Assert.IsTrue(transform.childCount > 0, $"Transform: {transform.name} hasnt child.");



    private void CreateMultFakeAudioResourceDataAndAddToList(string[] keys)
    {
        foreach (var key in keys)
            CreateFakeAudioResourceDataAndAddToList(key);
    }

    private void CreateFakeAudioResourceDataAndAddToList(string key)
    {
        var audioPrefab = CreateAudioPrefab(key);
        var fakeAudioResourceData = new FakeAudioResourceData(key, audioPrefab);
        _fakeAudioResourceDatas.Add(fakeAudioResourceData);
    }

    private GameObject CreateAudioPrefab(string key)
    {
        var audioPrefab = new GameObject(key);
        audioPrefab.AddComponent<AudioSource>();
        return audioPrefab;
    }
}

public class FakeAudioResourceData : IAudioResourceData
{
    //public variable
    public string Key { get; }
    public GameObject Prefab { get; }

    //public method
    public FakeAudioResourceData(string key, GameObject prefab)
    {
        Key = key;
        Prefab = prefab;
    }
}


public class FakeAudioModuleConfig : IAudioModuleConfig
{
    public string PoolRootName { get; private set; } = "[AudioModule]";
    public string PoolPrefix { get; private set; } = "[";
    public string PoolSuffix { get; private set; } = "s]";

    public AudioMixer AudioMixer { get; private set; }

    public string MasterVolumeParameter { get; private set; }
    public string BgmVolumeParameter { get; private set; }
    public string SfxVolumeParameter { get; private set; }
    public string VoiceVolumeParameter { get; private set; }


    public void SetPoolRootName(string poolRootName) => PoolRootName = poolRootName;
    public void SetPoolPrefix(string poolPrefix) => PoolPrefix = poolPrefix;
    public void SetPoolSuffix(string poolSuffix) => PoolSuffix = poolSuffix;

    public void SetAudioMixer(AudioMixer audioMixer) => AudioMixer = audioMixer;

    public void SetMasterVolumeParameter(string masterVolumeParameter) => MasterVolumeParameter = masterVolumeParameter;
    public void SetBgmVolumeParameter(string bgmVolumeParameter) => BgmVolumeParameter = bgmVolumeParameter;
    public void SetSfxVolumeParameter(string sfxVolumeParameter) => SfxVolumeParameter = sfxVolumeParameter;
    public void SetVoiceVolumeParameter(string voiceVolumeParameter) => VoiceVolumeParameter = voiceVolumeParameter;
}