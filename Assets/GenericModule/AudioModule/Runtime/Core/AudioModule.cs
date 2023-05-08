using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace CizaAudioModule
{
	public class AudioModule
	{
		//private variable
		private readonly IAudioModuleConfig        _config;
		private readonly IAudioModuleAssetProvider _assetProvider;

		private          Transform                     _poolRootTransform;
		private readonly Dictionary<string, Transform> _poolTransformMap = new();

		private readonly Dictionary<string, IAudio>       _playingAudioMap    = new();
		private readonly Dictionary<string, List<IAudio>> _unplayingAudiosMap = new();

		private IReadOnlyDictionary<string, IAudioData> _audioDataMap;

		//public variable
		public AudioMixer AudioMixer    { get; }
		public bool       IsInitialized => _audioDataMap != null &&_poolRootTransform != null;

		public string[] ClipDataIds   { get; private set; }
		public string[] PrefabDataIds { get; private set; }

		//public method
		public AudioModule(IAudioModuleConfig config, IAudioModuleAssetProvider assetProvider, AudioMixer audioMixer = null)
		{
			_config        = config;
			_assetProvider = assetProvider;
			Assert.IsNotNull(_config, $"[AudioModule::AudioModule] {nameof(IAudioModuleConfig)} is null.");
			Assert.IsNotNull(_assetProvider, $"[AudioModule::AudioModule] {nameof(assetProvider)} is null.");

			AudioMixer = audioMixer;
		}

		public void Initialize()
		{
			if (IsInitialized)
			{
				Debug.LogWarning("[AudioModule::Initialize] AudioModule is initialized.");
				return;
			}

			_audioDataMap = _config.CreateAudioDataMap();
			
			InitializeClipAndPrefabAndAssetDataIds();

			if (_poolRootTransform is null)
			{
				var poolRootGameObject = new GameObject(_config.PoolRootName);
				_poolRootTransform = poolRootGameObject.transform;
			}
		}

		public void Release()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioModule::Dispose] AudioModule is not initialized.");
				return;
			}

			StopAll();
			ReleaseUnplayingAudios();

			var prefabDataIds = _poolTransformMap.Keys.ToArray();
			foreach (var prefabDataId in prefabDataIds)
				DestroyPool(prefabDataId);

			var poolRootGameObject = _poolRootTransform.gameObject;
			_poolRootTransform = null;

			DestroyOrImmediate(poolRootGameObject);

			_audioDataMap = null;
			
			ClipDataIds   = null;
			PrefabDataIds = null;
		}

		public bool TryGetAudioReadModel(string audioId, out IAudioReadModel audioReadModel)
		{
			audioReadModel = null;
			if (!_playingAudioMap.ContainsKey(audioId))
				return false;

			audioReadModel = _playingAudioMap[audioId];
			return true;
		}

		public bool CheckIsPlaying(string audioId)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioModule::CheckIsPlaying] AudioModule is not initialized.");
				return false;
			}

			return _playingAudioMap.ContainsKey(audioId);
		}

		public void SetAudioMixerVolume(float volume)
		{
			if (AudioMixer is null)
			{
				Debug.LogWarning("[AudioModule::SetAudioMixerVolume] AudioMixer is null.");
				return;
			}

			SetAudioMixerFloat(_config.AudioMixerVolumeParameter, volume);
		}

		public bool CheckIsLoad(string clipDataId)
		{
			var audioData = _audioDataMap[clipDataId];
			return _assetProvider.CheckIsLoad<AudioClip>(audioData.ClipDataId) && _assetProvider.CheckIsLoad<GameObject>(audioData.PrefabDataId);
		}

		public async UniTask LoadAudio(string clipDataId, CancellationToken cancellationToken = default)
		{
			var audioData = _audioDataMap[clipDataId];
			var uniTasks  = new List<UniTask> { _assetProvider.LoadAsset<AudioClip>(audioData.ClipDataId), _assetProvider.LoadAsset<GameObject>(audioData.PrefabDataId) };
			try
			{
				await UniTask.WhenAll(uniTasks).AttachExternalCancellation(cancellationToken);
			}
			catch
			{
				Debug.LogWarning("[AudioModule::LoadAudio] Audio is canceled loading asset.");
			}
		}

		public bool TryPlay(string clipDataId, out string audioId, Vector3 position = default, Transform parentTransform = null, float volume = 1, bool isLocalPosition = false)
		{
			audioId = string.Empty;
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioModule::TryPlay] AudioModule is not initialized.");
				return false;
			}

			var audioData = _audioDataMap[clipDataId];
			var isPlay    = TryPlay(audioData, out audioId, position, parentTransform, volume, isLocalPosition);
			return isPlay;
		}

		public bool TryPlay(IAudioData audioData, out string audioId, Vector3 position = default, Transform parentTransform = null, float volume = 1, bool isLocalPosition = false)
		{
			audioId = string.Empty;

			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioModule::TryPlay] AudioModule is not initialized.");
				return false;
			}

			var clipDataId   = audioData.ClipDataId;
			var prefabDataId = audioData.PrefabDataId;
			var spatialBlend = audioData.SpatialBlend;

			if (!CheckIsLoad(clipDataId))
			{
				Debug.LogWarning($"[AudioModule::TryPlay] ClipDataId: {clipDataId} is unloaded.");
				return false;
			}

			var audio     = GetOrCreateAudio(prefabDataId);
			var audioClip = _assetProvider.GetAsset<AudioClip>(clipDataId);

			audioId = Guid.NewGuid().ToString();
			AddAudioToPlayingAudiosMap(audioId, audio, position, parentTransform, isLocalPosition);

			audio.Play(audioId, clipDataId, audioClip, spatialBlend, volume);
			audio.GameObject.name = clipDataId;

			return true;
		}

		public void Stop(string audioId)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioModule::Stop] AudioModule is not initialized.");
				return;
			}

			var audio = _playingAudioMap[audioId];
			audio.Stop();

			var clipDataId   = audio.ClipDataId;
			var prefabDataId = audio.PrefabDataId;

			_assetProvider.UnloadAsset(clipDataId);
			_assetProvider.UnloadAsset(prefabDataId);

			_playingAudioMap.Remove(audioId);
			AddAudioToUnplayingAudiosMap(audio);
		}

		public void StopAll()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioModule::StopAll] AudioModule is not initialized.");
				return;
			}

			var audioIds = _playingAudioMap.Keys.ToArray();
			foreach (var audioId in audioIds)
				Stop(audioId);
		}

		public void ReleaseUnplayingAudios()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioModule::ReleaseUnplayingAudios] AudioModule is not initialized.");
				return;
			}

			var prefabDataIds = _unplayingAudiosMap.Keys.ToArray();
			foreach (var prefabDataId in prefabDataIds)
				ReleaseUnplayingAudios(prefabDataId);
		}

		public void Resume(string audioId)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioModule::Resume] AudioModule is not initialized.");
				return;
			}

			if (!_playingAudioMap.ContainsKey(audioId))
			{
				Debug.LogWarning($"[AudioModule::Resume] Audio is not found by audioId: {audioId}.");
				return;
			}

			var usingAudio = _playingAudioMap[audioId];
			usingAudio.Resume();
		}

		public void Pause(string audioId)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioModule::Pause] AudioModule is not initialized.");
				return;
			}

			if (!_playingAudioMap.ContainsKey(audioId))
			{
				Debug.LogWarning($"[AudioModule::Pause] Audio is not found by audioId: {audioId}.");
				return;
			}

			var usingAudio = _playingAudioMap[audioId];
			usingAudio.Pause();
		}

		public void SetVolume(string audioId, float volume)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioModule::SetVolume] AudioModule is not initialized.");
				return;
			}

			if (!_playingAudioMap.ContainsKey(audioId))
			{
				Debug.LogWarning($"[AudioModule::SetVolume] Audio is not found by audioId: {audioId}.");
				return;
			}

			var usingAudio = _playingAudioMap[audioId];
			usingAudio.SetVolume(volume);
		}

		public string GetPoolName(string prefabDataId) =>
			_config.PoolPrefix + prefabDataId + _config.PoolSuffix;

		// private method

		private IAudio GetOrCreateAudio(string prefabDataId)
		{
			if (!_unplayingAudiosMap.ContainsKey(prefabDataId))
				CreatePool(prefabDataId);

			var unplayingAudios = _unplayingAudiosMap[prefabDataId];
			if (unplayingAudios.Count <= 0)
				CreateAudioAndAddToUnplayingAudiosMap(prefabDataId);

			var unplayingAudio = unplayingAudios.First();
			unplayingAudios.Remove(unplayingAudio);
			return unplayingAudio;
		}

		private void CreatePool(string prefabDataId)
		{
			_unplayingAudiosMap.Add(prefabDataId, new List<IAudio>());

			var poolName      = GetPoolName(prefabDataId);
			var pool          = new GameObject(poolName);
			var poolTransform = pool.transform;
			poolTransform.SetParent(_poolRootTransform);

			_poolTransformMap.Add(prefabDataId, poolTransform);
		}

		private void DestroyPool(string prefabDataId)
		{
			var poolTransform  = _poolTransformMap[prefabDataId];
			var poolGameObject = poolTransform.gameObject;
			DestroyOrImmediate(poolGameObject);
			_poolTransformMap.Remove(prefabDataId);
		}

		private void CreateAudioAndAddToUnplayingAudiosMap(string prefabDataId)
		{
			var audioPrefab = _assetProvider.GetAsset<GameObject>(prefabDataId);
			var audio       = Object.Instantiate(audioPrefab).GetComponent<IAudio>();
			audio.Initialize(prefabDataId);

			AddAudioToUnplayingAudiosMap(audio);
		}

		private void AddAudioToUnplayingAudiosMap(IAudio audio)
		{
			audio.GameObject.name = audio.PrefabDataId;
			var poolTransform = _poolTransformMap[audio.PrefabDataId];
			SetAudioTransform(audio, false, Vector3.zero, poolTransform, true);

			var unplayingAudios = _unplayingAudiosMap[audio.PrefabDataId];
			unplayingAudios.Add(audio);
		}

		private void AddAudioToPlayingAudiosMap(string audioId, IAudio audio, Vector3 position, Transform parentTransform, bool isLocalPosition)
		{
			SetAudioTransform(audio, true, position, parentTransform, isLocalPosition);

			_playingAudioMap.Add(audioId, audio);
		}

		private void SetAudioTransform(IAudio audio, bool isActive, Vector3 position, Transform parentTransform, bool isLocalPosition)
		{
			var audioGameObject = audio.GameObject;
			audioGameObject.SetActive(isActive);

			var audioTransform = audioGameObject.transform;

			audioTransform.SetParent(parentTransform);

			if (isLocalPosition)
				audioTransform.localPosition = position;
			else
				audioTransform.position = position;
		}

		private void SetAudioMixerFloat(string volumeParameter, float volume)
		{
			var linearToLogarithmicScale = GetLinearToLogarithmicScale(volume);
			AudioMixer.SetFloat(volumeParameter, linearToLogarithmicScale);
		}

		private float GetLinearToLogarithmicScale(float value) =>
			Mathf.Log(Mathf.Clamp(value, 0.001f, 1)) * 20.0f;

		private void InitializeClipAndPrefabAndAssetDataIds()
		{
			var audioDatas    = _audioDataMap.Values.ToArray();
			var clipDataIds   = new List<string>();
			var prefabDataIds = new List<string>();

			ForeachAudioDatas(audioDatas, audioData =>
			{
				clipDataIds.Add(audioData.ClipDataId);
				prefabDataIds.Add(audioData.PrefabDataId);
			});

			ClipDataIds   = clipDataIds.ToArray();
			PrefabDataIds = prefabDataIds.ToArray();
		}

		private void ForeachAudioDatas(IAudioData[] audioDatas, Action<IAudioData> action)
		{
			foreach (var audioData in audioDatas)
				action(audioData);
		}

		private void ReleaseUnplayingAudios(string prefabDataId)
		{
			if (!_unplayingAudiosMap.ContainsKey(prefabDataId))
				return;

			var unplayingAudios = _unplayingAudiosMap[prefabDataId];
			var audios          = unplayingAudios.ToArray();

			unplayingAudios.Clear();
			foreach (var audio in audios)
				DestroyOrImmediate(audio.GameObject);
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
