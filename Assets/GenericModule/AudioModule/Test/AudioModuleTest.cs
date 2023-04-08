using System.Collections.Generic;
using System.Linq;
using CizaAudioModule;
using NUnit.Framework;
using UnityEngine;


public class AudioModuleTest
{
    private FakeAudioModuleConfig _fakeAudioModuleConfig;
    private CizaAudioModule.AudioModule _audioModule;

    private List<FakeAudioData> _fakeAudioResourceDatas;

    private Transform _audioPlayingTransform;

    [SetUp]
    public void SetUp()
    {
        _fakeAudioModuleConfig = new FakeAudioModuleConfig();
        _audioModule = new CizaAudioModule.AudioModule(_fakeAudioModuleConfig);

        _fakeAudioResourceDatas = new List<FakeAudioData>();

        var audioPlayingGameObject = new GameObject();
        _audioPlayingTransform = audioPlayingGameObject.transform;
    }


    [Test]
    public void _01_Initialize()
    {
        //arrange
        Assert.IsFalse(_audioModule.IsInitialized);
        var fakeAudioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioData>();


        //act
        _audioModule.Initialize(fakeAudioResourceDatas);


        //assert
        Assert.IsTrue(_audioModule.IsInitialized, "AudioModule initialize success.");
    }

    [Test]
    public void _02_Release()
    {
        //arrange
        Assert.IsFalse(_audioModule.IsInitialized);

        var fakeAudioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioData>();
        _audioModule.Initialize(fakeAudioResourceDatas);
        Assert.IsTrue(_audioModule.IsInitialized);


        //act
        _audioModule.Release();


        //assert
        Assert.IsFalse(_audioModule.IsInitialized, "AudioModule release failed.");
    }

    [Test]
    public void _03_ReleaseAllPools()
    {
        //arrange
        var key1 = "HelloAudio_1";
        var key2 = "HelloAudio_2";
        var keys = new List<string>() { key1, key2 }.ToArray();
        CreateMultFakeAudioResourceDataAndAddToList(keys);

        var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioData>();
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
    public void _04_ReleasePool_By_Key()
    {
        //arrange
        var key1 = "HelloAudio_1";
        var key2 = "HelloAudio_2";
        var keys = new List<string>() { key1, key2 }.ToArray();
        CreateMultFakeAudioResourceDataAndAddToList(keys);

        var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioData>();
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
    public void _05_GetPoolName_By_Key()
    {
        //arrange
        var key = "HelloAudio_1";
        CreateFakeAudioResourceDataAndAddToList(key);

        var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioData>();
        Check_AudioModule_Already_Initialize(audioResourceDatas);


        //act
        var poolName = _audioModule.GetPoolName(key);


        //assert
        var expectedPoolName = _fakeAudioModuleConfig.PoolPrefix + key + _fakeAudioModuleConfig.PoolSuffix;
        Assert.AreEqual(expectedPoolName, poolName, "Pool name is not equal.");
    }

    [Test]
    public void _06_CheckIsPlaying_By_Key()
    {
        //arrange
        var key = "HelloAudio_1";
        CreateFakeAudioResourceDataAndAddToList(key);

        var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioData>();
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

        var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioData>();
        Check_AudioModule_Already_Initialize(audioResourceDatas);

        var id = _audioModule.Play(key);


        //act
        var audioData = _audioModule.GetAudio(id);


        //assert
        Assert.IsNotNull(audioData, "AudioData is null.");
    }


    [Test]
    public void _08_Play_By_Key_ParentTransform()
    {
        //arrange
        var key = "HelloAudio_1";
        CreateFakeAudioResourceDataAndAddToList(key);

        var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioData>();
        Check_AudioModule_Already_Initialize(audioResourceDatas);

        Check_AudioPlayingTransform_Hasnt_Child();


        //act
        var id = _audioModule.Play(key, _audioPlayingTransform);


        //assert
        Check_AudioPlayingTransform_Has_Child();
    }

    [Test]
    public void _09_Stop_By_Id()
    {
        //arrange
        var key = "HelloAudio_1";
        CreateFakeAudioResourceDataAndAddToList(key);

        var audioResourceDatas = _fakeAudioResourceDatas.ToArray<IAudioData>();
        Check_AudioModule_Already_Initialize(audioResourceDatas);

        Check_AudioPlayingTransform_Hasnt_Child();

        var id = _audioModule.Play(key, _audioPlayingTransform);
        Check_AudioPlayingTransform_Has_Child();


        //act
        _audioModule.Stop(id);


        //assert
        Check_AudioPlayingTransform_Hasnt_Child();
    }


    private void Check_AudioModule_Already_Initialize(IAudioData[] audioResourceDatas)
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


    private void Check_AudioPlayingTransform_Hasnt_Child() =>
        Assert.IsTrue(_audioPlayingTransform.childCount == 0, $"AudioPlayingTransform has child.");


    private void Check_AudioPlayingTransform_Has_Child() =>
        Assert.IsTrue(_audioPlayingTransform.childCount > 0, $"AudioPlayingTransform hasnt child.");


    private void CreateMultFakeAudioResourceDataAndAddToList(string[] keys)
    {
        foreach (var key in keys)
            CreateFakeAudioResourceDataAndAddToList(key);
    }

    private void CreateFakeAudioResourceDataAndAddToList(string key)
    {
        var audioPrefab = CreateAudioPrefab(key);
        var fakeAudioResourceData = new FakeAudioData(key, audioPrefab);
        _fakeAudioResourceDatas.Add(fakeAudioResourceData);
    }

    private GameObject CreateAudioPrefab(string key)
    {
        var audioPrefab = new GameObject(key);
        audioPrefab.AddComponent<AudioSource>();
        return audioPrefab;
    }
}