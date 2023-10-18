using System.Collections.Generic;
using CizaAudioModule;
using NUnit.Framework;
using UnityEngine;

// public class AudioModuleTest
// {
// 	private FakeAudioModuleConfig        _fakeAudioModuleConfig;
// 	private FakeAssetProvider _fakeAssetProvider;
//
// 	private AudioModule _audioModule;
// 	private Transform   _parentTransform;
//
// 	[SetUp]
// 	public void SetUp()
// 	{
// 		_fakeAssetProvider = new FakeAssetProvider();
// 		_fakeAudioModuleConfig        = new FakeAudioModuleConfig();
// 		_fakeAudioModuleConfig.SetAudioDataMap(new Dictionary<string, IAudioInfo>() { { FakeAssetProvider.CLIP_DATA_ID, new FakeAudioInfo(FakeAssetProvider.CLIP_DATA_ID, FakeAssetProvider.PREFAB_DATA_ID, FakeAssetProvider.SPATIAL_BLEND) } });
//
// 		_audioModule     = new AudioModule(_fakeAudioModuleConfig, _fakeAssetProvider);
// 		_parentTransform = new GameObject().transform;
// 	}
//
// 	[Test]
// 	public void _01_IsInitialized_Is_True_After_AudioModule_Is_Initialized()
// 	{
// 		// arrange
// 		Assert.IsFalse(_audioModule.IsInitialized);
// 		Assert.IsNull(_fakeAssetProvider.LoadedAssetsDataIds);
//
// 		// act
// 		_audioModule.Initialize();
//
//
// 		// assert
// 		Assert.IsTrue(_audioModule.IsInitialized, "AudioModule is not initialized.");
// 		Check_String_Array_Content_Is_Equal(FakeAssetProvider.ASSETS_DATA_IDS, _fakeAssetProvider.LoadedAssetsDataIds, "AssetsDataIds is incorrect.");
// 	}
//
// 	[Test]
// 	public void _02_IsInitialized_Is_False_After_AudioModule_Is_Disposed()
// 	{
// 		// arrange
// 		_01_IsInitialized_Is_True_After_AudioModule_Is_Initialized();
//
//
// 		// act
// 		_audioModule.Release();
//
//
// 		// assert
// 		Assert.IsFalse(_audioModule.IsInitialized, "AudioModule is initialized.");
// 		Check_String_Array_Content_Is_Equal(FakeAssetProvider.ASSETS_DATA_IDS, _fakeAssetProvider.UnloadedAssetsDataIds, "AssetsDataIds is incorrect.");
// 	}
//
// 	[Test]
// 	public void _03_Instantiate_Pool_And_Audio_After_AudioModule_Is_Played()
// 	{
// 		// arranges
// 		_01_IsInitialized_Is_True_After_AudioModule_Is_Initialized();
//
// 		var fakeAudioClip = CreateFakeAudioClip(FakeAssetProvider.CLIP_DATA_ID);
// 		_fakeAssetProvider.SetClip(fakeAudioClip);
//
// 		var fakeAudioPrefab = CreateFakeAudioPrefab(FakeAssetProvider.PREFAB_DATA_ID);
// 		_fakeAssetProvider.SetAudioPrefab(fakeAudioPrefab);
//
// 		var localPosition = new Vector3(0, 1, 0);
// 		var volume        = 0.6f;
//
//
// 		// act
// 		var isPlay = _audioModule.TryPlay(FakeAssetProvider.CLIP_DATA_ID, out var  audioId, localPosition, _parentTransform, volume);
//
//
// 		// assert
// 		Assert.IsTrue(_parentTransform.childCount == 1, "Audio is not instantiated.");
//
// 		var audio = _parentTransform.GetComponentInChildren<IAudio>();
// 		Assert.AreEqual(audio.Volume, volume, "Audio volume is incorrect.");
//
// 		var audioGameObject = audio.GameObject;
// 		var audioTransform  = audioGameObject.transform;
// 		Assert.AreEqual(audioTransform.position, localPosition, "Audio position is incorrect.");
//
// 		Assert.IsTrue(_audioModule.CheckIsPlaying(audioId), "Audio is not playing.");
// 		Assert.IsTrue(audioGameObject.activeSelf, "Audio is not Active.");
// 	}
//
// 	[Test]
// 	public void _04_Audio_Stop_Playing_Clip_And_Return_To_Pool_After_AudioModule_Is_stopped()
// 	{
// 		// arrange
// 		_03_Instantiate_Pool_And_Audio_After_AudioModule_Is_Played();
// 		var audio   = _parentTransform.GetComponentInChildren<IAudio>();
// 		var audioId = audio.Id;
//
//
// 		// act
// 		_audioModule.Stop(audio.Id);
//
//
// 		// assert
// 		Assert.IsFalse(_audioModule.CheckIsPlaying(audioId), "Audio is playing.");
//
// 		var audioGameObject = audio.GameObject;
// 		var audioTransform  = audioGameObject.transform;
// 		var poolTransform   = audioTransform.parent;
// 		Assert.AreEqual(_audioModule.GetPoolName(FakeAssetProvider.PREFAB_DATA_ID), poolTransform.name, "PoolName is incorrect.");
// 		Assert.IsFalse(audioGameObject.activeSelf, "Audio is Active.");
// 	}
//
// 	[TestCase(1)]
// 	[TestCase(0.5f)]
// 	public void _05_Set_Audio_Volume_After_AudioModule_Is_Called_SetVolume(float volume)
// 	{
// 		// arrange
// 		_03_Instantiate_Pool_And_Audio_After_AudioModule_Is_Played();
// 		var audio   = _parentTransform.GetComponentInChildren<IAudio>();
// 		var audioId = audio.Id;
//
// 		// act
// 		_audioModule.SetVolume(audioId, volume);
//
//
// 		// assert
// 		Assert.AreEqual(volume, audio.Volume, "Audio volume is incorrect.");
// 	}
//
// 	// private method
// 	private Dictionary<string, IAudioInfo> CreateAudioDataMap(IAudioInfo[] audioDatas)
// 	{
// 		var audioDataMap = new Dictionary<string, IAudioInfo>();
// 		foreach (var audioData in audioDatas)
// 			audioDataMap.Add(audioData.ClipAddress, audioData);
//
// 		return audioDataMap;
// 	}
//
// 	private void Check_String_Array_Content_Is_Equal(string[] expectedStrs, string[] strs, string message)
// 	{
// 		for (int i = 0; i < expectedStrs.Length; i++)
// 		{
// 			var expectedStr = expectedStrs[i];
// 			var str         = strs[i];
//
// 			Assert.AreEqual(expectedStr, str, message);
// 		}
// 	}
//
// 	private AudioClip CreateFakeAudioClip(string name)
// 	{
// 		var frequency = 1000;
// 		var audioClip = AudioClip.Create(name, frequency * 2, 1, frequency, true);
// 		return audioClip;
// 	}
//
// 	private GameObject CreateFakeAudioPrefab(string name)
// 	{
// 		var fakeAudioPrefab = new GameObject(name);
// 		fakeAudioPrefab.AddComponent<Audio>();
// 		fakeAudioPrefab.AddComponent<AudioSource>();
// 		return fakeAudioPrefab;
// 	}
// }
