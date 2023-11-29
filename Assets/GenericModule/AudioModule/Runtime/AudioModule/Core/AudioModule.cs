using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CizaTimerModule;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace CizaAudioModule
{
	public class AudioModule
	{
		private readonly IAudioModuleConfig _config;
		private readonly IAssetProvider     _clipAssetProvider;
		private readonly IAssetProvider     _prefabAssetProvider;
		private readonly AudioMixer         _audioMixer;
		private readonly bool               _isDontDestroyOnLoad;

		private readonly TimerModule                _timerModule         = new TimerModule();
		private readonly Dictionary<string, string> _timerIdMapByAudioId = new Dictionary<string, string>();

		private readonly Dictionary<string, Transform> _poolMapByPrefabAddress = new Dictionary<string, Transform>();

		private readonly Dictionary<string, int> _loadedCountMapByDataId = new Dictionary<string, int>();

		private readonly List<string> _loadedClipAddresses   = new List<string>();
		private readonly List<string> _loadedPrefabAddresses = new List<string>();

		private readonly Dictionary<string, AudioClip>  _clipMapByAddress   = new Dictionary<string, AudioClip>();
		private readonly Dictionary<string, GameObject> _prefabMapByAddress = new Dictionary<string, GameObject>();

		private readonly Dictionary<string, List<IAudio>> _idleAudioMapByPrefabAddress = new Dictionary<string, List<IAudio>>();
		private readonly Dictionary<string, IAudio>       _playingAudioMapByAudioId    = new Dictionary<string, IAudio>();

		private Transform                               _poolRoot;
		private IReadOnlyDictionary<string, IAudioInfo> _audioInfoMapByDataId;

		// Id, DataId
		public event Action<string, string> OnPlay;
		public event Action<string, string> OnStop;
		public event Action<string, string> OnComplete;

		public bool IsInitialized => _audioInfoMapByDataId != null && _poolRoot != null;

		public string[] AudioDataIds => _audioInfoMapByDataId != null ? _audioInfoMapByDataId.Keys.ToArray() : Array.Empty<string>();

		public bool TryGetAudioMixerGroup(out AudioMixerGroup audioMixerGroup)
		{
			if (_audioMixer is null)
			{
				Debug.LogWarning("[AudioModule::TryGetAudioMixerGroup] AudioMixer is null.");
				audioMixerGroup = null;
				return false;
			}

			audioMixerGroup = _audioMixer.FindMatchingGroups(_config.AudioMixerGroupPath).First();
			return audioMixerGroup != null;
		}

		public bool TryGetVolume(out float volume)
		{
			if (_audioMixer is null)
			{
				Debug.LogWarning("[AudioModule::TryGetVolume] AudioMixer is null.");
				volume = 0;
				return false;
			}

			return _audioMixer.GetFloat(_config.AudioMixerParameter, out volume);
		}

		public bool CheckIsAudioInfoLoaded(string audioDataId) =>
			CheckIsAudioInfoLoaded(audioDataId, "CheckIsAudioInfoLoaded", out var clipAddress, out var prefabAddress);

		//public method
		public AudioModule(IAudioModuleConfig config, IAssetProvider clipAssetProvider, IAssetProvider prefabAssetProvider, AudioMixer audioMixer, bool isDontDestroyOnLoad = false)
		{
			_config              = config;
			_clipAssetProvider   = clipAssetProvider;
			_prefabAssetProvider = prefabAssetProvider;
			Assert.IsNotNull(_config, $"[AudioModule::AudioModule] {nameof(IAudioModuleConfig)} is null.");
			Assert.IsNotNull(_clipAssetProvider, $"[AudioModule::AudioModule] ClipAssetProvider is null.");
			Assert.IsNotNull(_prefabAssetProvider, $"[AudioModule::AudioModule] PrefabAssetProvider is null.");

			_audioMixer          = audioMixer;
			_isDontDestroyOnLoad = isDontDestroyOnLoad;

			_timerModule.OnRemove += m_OnRemove;

			OnStop += m_OnStop;

			void m_OnRemove(string timerId)
			{
				foreach (var m_pair in _timerIdMapByAudioId.Where(m_pair => m_pair.Value == timerId).ToArray())
					_timerIdMapByAudioId.Remove(m_pair.Key);
			}

			void m_OnStop(string audioId, string audioDataId) =>
				RemoveTimer(audioId);
		}

		public void Initialize(Transform rootParent = null)
		{
			if (IsInitialized)
			{
				Debug.LogWarning("[AudioModule::Initialize] AudioModule is initialized.");
				return;
			}

			_timerModule.Initialize();

			_audioInfoMapByDataId = _config.CreateAudioInfoMapDataId();

			if (_poolRoot is null)
			{
				var poolRootGameObject = new GameObject(_config.PoolRootName);
				if (_isDontDestroyOnLoad)
					Object.DontDestroyOnLoad(poolRootGameObject);

				_poolRoot = poolRootGameObject.transform;

				if (rootParent != null)
					_poolRoot.SetParent(rootParent);
			}

			SetVolume(_config.DefaultVolume);
		}

		public async void Release()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioModule::Release] AudioModule is not initialized.");
				return;
			}

			foreach (var pair in _loadedCountMapByDataId.ToArray())
			{
				var audioDataId = pair.Key;
				for (var i = 0; i < pair.Value; i++)
					UnloadAudioAsset(audioDataId);
			}

			var poolRootGameObject = _poolRoot.gameObject;
			_poolRoot = null;
			DestroyOrImmediate(poolRootGameObject);

			_audioInfoMapByDataId = null;

			_timerModule.Release();
		}

		public async void Tick(float deltaTime)
		{
			if (!IsInitialized)
				return;

			_timerModule.Tick(deltaTime);

			foreach (var audio in _playingAudioMapByAudioId.Values.ToArray())
			{
				if (audio.IsComplete)
				{
					if (audio.IsLoop)
					{
						audio.Continue();
						continue;
					}

					await StopAsync(audio.Id, 0);
					OnComplete?.Invoke(audio.DataId, audio.Id);
					continue;
				}

				audio.Tick(deltaTime);
			}
		}

		public bool TryGetAudioReadModel(string audioId, out IAudioReadModel audioReadModel)
		{
			if (!IsInitialized)
			{
				audioReadModel = null;
				Debug.LogWarning("[AudioModule::TryGetAudioReadModel] AudioModule is not initialized.");
				return false;
			}

			if (!_playingAudioMapByAudioId.ContainsKey(audioId))
			{
				audioReadModel = null;
				return false;
			}

			audioReadModel = _playingAudioMapByAudioId[audioId];
			return true;
		}

		public bool CheckIsPlaying(string audioId)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioModule::CheckIsPlaying] AudioModule is not initialized.");
				return false;
			}

			return _playingAudioMapByAudioId.ContainsKey(audioId);
		}

		public void SetVolume(float volume)
		{
			if (_audioMixer is null)
			{
				Debug.LogWarning("[AudioModule::SetAudioMixerVolume] AudioMixer is null.");
				return;
			}

			_audioMixer.SetFloat(_config.AudioMixerParameter, m_GetLinearToLogarithmicScale(volume));

			float m_GetLinearToLogarithmicScale(float value) =>
				Mathf.Log(Mathf.Clamp(value, 0.001f, 1)) * 20.0f;
		}

		public async UniTask LoadAudioAssetAsync(string audioDataId, CancellationToken cancellationToken = default)
		{
			if (!_audioInfoMapByDataId.TryGetValue(audioDataId, out var audioInfo))
			{
				Debug.LogError($"[AudioModule::LoadAudioAssetAsync] Not find audioInfo by dataId: {audioDataId}. Please check AudioModule config.");
				return;
			}

			if (_loadedCountMapByDataId.ContainsKey(audioDataId))
			{
				_loadedCountMapByDataId[audioDataId]++;
				return;
			}

			_loadedCountMapByDataId.Add(audioDataId, 1);

			var clipAddress = audioInfo.ClipAddress;
			Assert.IsTrue(HasValue(clipAddress), $"[AudioModule::LoadAudioAssetAsync] AudioDataId - {audioDataId}'s clipAddress is null.");

			var prefabAddress = HasValue(audioInfo.PrefabAddress) ? audioInfo.PrefabAddress : _config.DefaultPrefabAddress;
			Assert.IsTrue(HasValue(prefabAddress), $"[AudioModule::LoadAudioAssetAsync] AudioDataId - {audioDataId}'s prefabAddress is null.");

			var clip = await _clipAssetProvider.LoadAssetAsync<AudioClip>(clipAddress, cancellationToken);
			_clipMapByAddress.TryAdd(clipAddress, clip);
			_loadedClipAddresses.Add(clipAddress);

			var prefab = await _prefabAssetProvider.LoadAssetAsync<GameObject>(prefabAddress, cancellationToken);
			_prefabMapByAddress.TryAdd(prefabAddress, prefab);
			_loadedPrefabAddresses.Add(clipAddress);
		}

		public async void UnloadAudioAsset(string audioDataId)
		{
			if (!_audioInfoMapByDataId.TryGetValue(audioDataId, out var audioInfo))
			{
				Debug.LogError($"[AudioModule::UnloadAudioAsset] Not find audioInfo by dataId: {audioDataId}. Please check AudioModule config.");
				return;
			}

			if (!_loadedCountMapByDataId.ContainsKey(audioDataId))
				return;

			_loadedCountMapByDataId[audioDataId]--;
			if (_loadedCountMapByDataId[audioDataId] > 0)
				return;

			_loadedCountMapByDataId.Remove(audioDataId);

			await StopByDataIdAsync(audioDataId, 0);

			m_UnloadClip(audioInfo.ClipAddress);
			m_UnloadPrefab(audioInfo.PrefabAddress);

			void m_UnloadClip(string m_clipAddress)
			{
				if (_loadedClipAddresses.Contains(m_clipAddress))
				{
					_clipAssetProvider.UnloadAsset<AudioClip>(m_clipAddress);
					_loadedClipAddresses.Remove(m_clipAddress);
				}

				if (!_loadedClipAddresses.Contains(m_clipAddress) && _clipMapByAddress.ContainsKey(m_clipAddress))
					_clipMapByAddress.Remove(m_clipAddress);
			}

			void m_UnloadPrefab(string m_prefabAddress)
			{
				if (_loadedPrefabAddresses.Contains(m_prefabAddress))
				{
					_prefabAssetProvider.UnloadAsset<GameObject>(m_prefabAddress);
					_loadedPrefabAddresses.Remove(m_prefabAddress);
				}

				if (!_loadedPrefabAddresses.Contains(m_prefabAddress) && _prefabMapByAddress.ContainsKey(m_prefabAddress))
				{
					_prefabMapByAddress.Remove(m_prefabAddress);
					m_DestroyIdleAudioAndPool(m_prefabAddress);
				}
			}

			void m_DestroyIdleAudioAndPool(string m_prefabAddress)
			{
				if (!_idleAudioMapByPrefabAddress.ContainsKey(m_prefabAddress))
					return;

				foreach (var m_audio in _idleAudioMapByPrefabAddress[m_prefabAddress].ToArray())
					DestroyOrImmediate(m_audio.GameObject);
				_idleAudioMapByPrefabAddress.Remove(m_prefabAddress);


				var m_pool = _poolMapByPrefabAddress[m_prefabAddress];
				_poolMapByPrefabAddress.Remove(m_prefabAddress);
				DestroyOrImmediate(m_pool.gameObject);
			}
		}

		public async UniTask<string> PlayAsync(string audioDataId, float volume = 1, float fadeTime = 0, bool isLoop = false, Vector3 position = default)
		{
			if (!CheckIsAudioInfoLoaded(audioDataId, "PlayAsync", out var clipAddress, out var prefabAddress))
				return string.Empty;

			var audio = m_GetOrCreateAudio(prefabAddress);

			var audioId   = Guid.NewGuid().ToString();
			var audioClip = _clipMapByAddress[clipAddress];

			AddAudioToPlayingAudiosMap(audioId, audio, position);

			audio.GameObject.name = clipAddress;
			OnPlay?.Invoke(audioId, audioDataId);

			if (fadeTime > 0)
			{
				audio.Play(audioId, audioDataId, clipAddress, audioClip, 0, isLoop);
				await AddTimer(audioId, 0, volume, fadeTime);
			}
			else
				audio.Play(audioId, audioDataId, clipAddress, audioClip, volume, isLoop);

			return audio.Id;

			IAudio m_GetOrCreateAudio(string m_prefabAddress)
			{
				if (!_idleAudioMapByPrefabAddress.ContainsKey(m_prefabAddress))
					m_CreatePool(m_prefabAddress);

				var m_idleAudios = _idleAudioMapByPrefabAddress[m_prefabAddress];
				if (m_idleAudios.Count <= 0)
				{
					var m_prefab = _prefabMapByAddress[m_prefabAddress];
					var m_audio  = Object.Instantiate(m_prefab).GetComponent<IAudio>();
					m_audio.Initialize(prefabAddress);

					AddAudioToIdleAudiosMap(m_audio);
				}

				var m_idleAudio = m_idleAudios.First();
				m_idleAudios.Remove(m_idleAudio);
				return m_idleAudio;
			}

			void m_CreatePool(string m_prefabAddress)
			{
				_idleAudioMapByPrefabAddress.Add(m_prefabAddress, new List<IAudio>());

				var poolName      = mm_GetPoolName(m_prefabAddress);
				var pool          = new GameObject(poolName);
				var poolTransform = pool.transform;
				poolTransform.SetParent(_poolRoot);

				_poolMapByPrefabAddress.Add(m_prefabAddress, poolTransform);

				string mm_GetPoolName(string mm_prefabAddress) =>
					_config.PoolPrefix + mm_prefabAddress + _config.PoolSuffix;
			}
		}

		public async UniTask ModifyAsync(string audioId, float volume, bool isLoop, float time)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioModule::ModifyAsync] AudioModule is not initialized.");
				return;
			}

			if (!_playingAudioMapByAudioId.TryGetValue(audioId, out var playingAudio))
			{
				Debug.LogWarning($"[AudioModule::ModifyAsync] Audio is not found by audioId: {audioId}.");
				return;
			}

			playingAudio.SetIsLoop(isLoop);
			await ModifyAsync(audioId, volume, time);
		}

		public async UniTask ModifyAsync(string audioId, float volume, float time)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioModule::ModifyAsync] AudioModule is not initialized.");
				return;
			}

			if (!_playingAudioMapByAudioId.TryGetValue(audioId, out var playingAudio))
			{
				Debug.LogWarning($"[AudioModule::ModifyAsync] Audio is not found by audioId: {audioId}.");
				return;
			}

			if (time > 0)
				await AddTimer(audioId, playingAudio.Volume, volume, time);
			else
				playingAudio.SetVolume(volume);
		}

		public void Pause(string audioId)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioModule::Pause] AudioModule is not initialized.");
				return;
			}

			if (!_playingAudioMapByAudioId.TryGetValue(audioId, out var playingAudio))
			{
				Debug.LogWarning($"[AudioModule::Pause] Audio is not found by audioId: {audioId}.");
				return;
			}

			playingAudio.Pause();
		}

		public void Resume(string audioId)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioModule::Resume] AudioModule is not initialized.");
				return;
			}

			if (!_playingAudioMapByAudioId.TryGetValue(audioId, out var playingAudio))
			{
				Debug.LogWarning($"[AudioModule::Resume] Audio is not found by audioId: {audioId}.");
				return;
			}

			playingAudio.Resume();
		}

		public async UniTask StopAsync(string audioId, float fadeTime = 0)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioModule::StopAsync] AudioModule is not initialized.");
				return;
			}

			if (!_playingAudioMapByAudioId.TryGetValue(audioId, out var playingAudio))
			{
				Debug.LogWarning($"[AudioModule::Pause] Audio is not found by audioId: {audioId}.");
				return;
			}

			if (fadeTime > 0)
				await AddTimer(audioId, playingAudio.Volume, 0, fadeTime);

			OnStop?.Invoke(audioId, playingAudio.DataId);
			
			_playingAudioMapByAudioId.Remove(audioId);
			AddAudioToIdleAudiosMap(playingAudio);

			playingAudio.Stop();
		}

		public UniTask StopByDataIdAsync(string audioDataId, float fadeTime = 0)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[StopByDataIdAsync] AudioModule is not initialized.");
				return UniTask.CompletedTask;
			}

			var uniTasks = new List<UniTask>();

			foreach (var pair in _playingAudioMapByAudioId.Where(pair => pair.Value.DataId == audioDataId).ToArray())
				uniTasks.Add(StopAsync(pair.Key, fadeTime));

			return UniTask.WhenAll(uniTasks);
		}

		public async UniTask StopAllAsync(float fadeTime = 0)
		{
			var uniTasks = new List<UniTask>();
			foreach (var audioId in _playingAudioMapByAudioId.Keys.ToArray())
				uniTasks.Add(StopAsync(audioId, fadeTime));
			await UniTask.WhenAll(uniTasks);
		}

		private bool CheckIsAudioInfoLoaded(string audioDataId, string methodName, out string clipAddress, out string prefabAddress)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[AudioModule::{methodName}] AudioModule is not initialized.");
				clipAddress   = string.Empty;
				prefabAddress = string.Empty;
				return false;
			}

			if (!_loadedCountMapByDataId.ContainsKey(audioDataId))
			{
				Debug.LogWarning($"[AudioModule::CheckIsAudioInfoLoaded] AudioDataId - {audioDataId} is not loaded.");
				clipAddress   = string.Empty;
				prefabAddress = string.Empty;
				return false;
			}

			Assert.IsTrue(_audioInfoMapByDataId.ContainsKey(audioDataId), $"[AudioModule::{methodName}] Not find audioInfo by audioDataId - {audioDataId}.");
			var audioInfo = _audioInfoMapByDataId[audioDataId];

			clipAddress = audioInfo.ClipAddress;
			if (!_clipMapByAddress.ContainsKey(clipAddress))
			{
				Debug.LogWarning($"[AudioModule::{methodName}] AudioInfo - {audioDataId}'s ClipAddress: {clipAddress} is unloaded.");
				prefabAddress = string.Empty;
				return false;
			}

			prefabAddress = HasValue(audioInfo.PrefabAddress) ? audioInfo.PrefabAddress : _config.DefaultPrefabAddress;
			if (!_prefabMapByAddress.ContainsKey(prefabAddress))
			{
				Debug.LogWarning($"[AudioModule::{methodName}] AudioInfo - {audioDataId}'s PrefabAddress: {prefabAddress} is unloaded.");
				return false;
			}

			return true;
		}

		private void AddAudioToIdleAudiosMap(IAudio audio)
		{
			audio.GameObject.name = audio.PrefabAddress;
			var pool = _poolMapByPrefabAddress[audio.PrefabAddress];
			SetAudioTransform(audio, false, Vector3.zero, pool);

			var idleAudios = _idleAudioMapByPrefabAddress[audio.PrefabAddress];
			idleAudios.Add(audio);
		}

		private void AddAudioToPlayingAudiosMap(string audioId, IAudio audio, Vector3 position)
		{
			SetAudioTransform(audio, true, position, _poolRoot);
			_playingAudioMapByAudioId.Add(audioId, audio);
		}

		private void SetAudioTransform(IAudio audio, bool isActive, Vector3 position, Transform parent)
		{
			var audioGameObject = audio.GameObject;
			audioGameObject.SetActive(isActive);

			var audioTransform = audioGameObject.transform;

			audioTransform.SetParent(parent);
			audioTransform.position = position;
		}

		private void DestroyOrImmediate(Object obj)
		{
			if (Application.isPlaying)
				Object.Destroy(obj);
			else
				Object.DestroyImmediate(obj);
		}

		private bool HasValue(string value) =>
			!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value);

		private async UniTask AddTimer(string audioId, float startVolume, float endVolume, float duration)
		{
			if (_timerIdMapByAudioId.ContainsKey(audioId))
				RemoveTimer(audioId);

			var audio = _playingAudioMapByAudioId[audioId];

			var timerId = _timerModule.AddOnceTimer(startVolume, endVolume, duration, (ITimerReadModel, value) => { audio.SetVolume(value); });
			_timerIdMapByAudioId.Add(audioId, timerId);

			while (_timerIdMapByAudioId.ContainsValue(timerId))
				await UniTask.Yield();
		}

		private void RemoveTimer(string audioId)
		{
			if (!_timerIdMapByAudioId.TryGetValue(audioId, out var timerId))
				return;

			_timerModule.RemoveTimer(timerId);
			_timerIdMapByAudioId.Remove(audioId);
		}
	}
}
