using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CizaTimerModule;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace CizaAudioModule
{
	public class AudioModule
	{
		private readonly IAudioModuleConfig _config;
		private readonly IAssetProvider _clipAssetProvider;
		private readonly IAssetProvider _prefabAssetProvider;
		private readonly AudioMixer _audioMixer;
		private readonly bool _isDontDestroyOnLoad;

		private readonly TimerModule _timerModule = new TimerModule();
		private readonly Dictionary<string, string> _timerIdMapByAudioId = new Dictionary<string, string>();

		private readonly Dictionary<string, Transform> _poolMapByPrefabAddress = new Dictionary<string, Transform>();

		private readonly Dictionary<string, int> _loadedCountMapByDataId = new Dictionary<string, int>();

		private readonly List<string> _loadedClipAddresses = new List<string>();
		private readonly List<string> _loadedPrefabAddresses = new List<string>();

		private readonly Dictionary<string, AudioClip> _clipMapByAddress = new Dictionary<string, AudioClip>();
		private readonly Dictionary<string, GameObject> _prefabMapByAddress = new Dictionary<string, GameObject>();

		private readonly Dictionary<string, List<IAudio>> _idleAudioMapByPrefabAddress = new Dictionary<string, List<IAudio>>();
		private readonly Dictionary<string, IAudio> _playingAudioMapByAudioId = new Dictionary<string, IAudio>();

		private readonly Dictionary<string, int> _consecutiveCountMapByDataId = new Dictionary<string, int>();

		private Transform _poolRoot;
		private IReadOnlyDictionary<string, IAudioInfo> _audioInfoMapByDataId;

		// CallerId, Id, DataId
		public event Action<string, string, string> OnSpawn;

		public event Action<string, string, string> OnStop;

		public event Action<string, string, string> OnComplete;

		public bool IsInitialized => _audioInfoMapByDataId != null && _poolRoot != null;

		public string[] AudioDataIds => _audioInfoMapByDataId != null ? _audioInfoMapByDataId.Keys.ToArray() : Array.Empty<string>();

		public bool TryGetAudioMixerGroup(out AudioMixerGroup audioMixerGroup)
		{
			if (_audioMixer is null)
			{
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
				volume = 0;
				return false;
			}

			return _audioMixer.GetFloat(_config.AudioMixerParameter, out volume);
		}

		public bool CheckIsAudioInfoLoaded(string audioDataId) =>
			CheckIsAudioInfoLoaded(audioDataId, "CheckIsAudioInfoLoaded", out _, out _);

		public bool CheckIsOnCooldown(string audioDataId)
		{
			if (!_config.RestrictContinuousPlay.IsEnable || !_consecutiveCountMapByDataId.TryGetValue(audioDataId, out var count))
				return false;

			return count >= _config.RestrictContinuousPlay.MaxConsecutiveCount;
		}

		public bool TryGetAudioReadModel(string audioId, out IAudioReadModel audioReadModel)
		{
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
			if (!TryGetAudioReadModel(audioId, out var audioReadModel))
				return false;

			return audioReadModel.IsPlaying;
		}

		public bool CheckIsPause(string audioId)
		{
			if (!TryGetAudioReadModel(audioId, out var audioReadModel))
				return false;

			return !audioReadModel.IsPlaying;
		}

		public float GetDuration(string audioDataId)
		{
			if (_audioInfoMapByDataId.TryGetValue(audioDataId, out var audioInfo) && _clipMapByAddress.TryGetValue(audioInfo.ClipAddress, out var clip))
				return clip.length;
			return 0;
		}

		[Preserve]
		public AudioModule(IAudioModuleConfig config, IAssetProvider clipAssetProvider, IAssetProvider prefabAssetProvider, AudioMixer audioMixer, bool isDontDestroyOnLoad = false)
		{
			_config = config;
			_clipAssetProvider = clipAssetProvider;
			_prefabAssetProvider = prefabAssetProvider;
			Assert.IsNotNull(_config, $"[AudioModule::AudioModule] AudioModuleConfig is null.");
			Assert.IsNotNull(_clipAssetProvider, $"[AudioModule::AudioModule] ClipAssetProvider is null.");
			Assert.IsNotNull(_prefabAssetProvider, $"[AudioModule::AudioModule] PrefabAssetProvider is null.");

			_audioMixer = audioMixer;
			_isDontDestroyOnLoad = isDontDestroyOnLoad;

			_timerModule.OnRemove += m_OnRemove;

			OnStop += m_OnDeSpawn;

			void m_OnRemove(string timerId)
			{
				foreach (var m_pair in _timerIdMapByAudioId.Where(m_pair => m_pair.Value == timerId).ToArray())
					_timerIdMapByAudioId.Remove(m_pair.Key);
			}

			void m_OnDeSpawn(string callerId, string audioId, string audioDataId) =>
				RemoveTimer(audioId);
		}

		public void Initialize(Transform rootParent = null)
		{
			if (IsInitialized)
				return;

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

		public void Release()
		{
			if (!IsInitialized)
				return;

			foreach (var pair in _loadedCountMapByDataId.ToArray())
			{
				var audioDataId = pair.Key;
				for (var i = 0; i < pair.Value; i++)
					UnloadAsset(audioDataId);
			}

			var poolRootGameObject = _poolRoot.gameObject;
			_poolRoot = null;
			DestroyOrImmediate(poolRootGameObject);

			_audioInfoMapByDataId = null;

			_timerModule.Release();
			_consecutiveCountMapByDataId.Clear();
		}

		public void Tick(float deltaTime)
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

					if (!audio.HasUserId)
						DeSpawn(audio.Id, OnComplete);
					continue;
				}

				audio.Tick(deltaTime);
			}
		}

		public void SetVolume(float volume)
		{
			if (_audioMixer is null)
			{
				Debug.LogWarning("[AudioModule::SetVolume] AudioMixer is null.");
				return;
			}

			_audioMixer.SetFloat(_config.AudioMixerParameter, m_GetLinearToLogarithmicScale(volume));

			float m_GetLinearToLogarithmicScale(float value) =>
				Mathf.Log(Mathf.Clamp(value, 0.001f, 1)) * 20.0f;
		}

		public async UniTask LoadAssetAsync(string audioDataId, string errorMessage, CancellationToken cancellationToken = default)
		{
			if (!_audioInfoMapByDataId.TryGetValue(audioDataId, out var audioInfo))
			{
				Debug.LogError($"[AudioModule::LoadAssetAsync] AudioInfo is not find by dataId: {audioDataId}. Please check AudioModule config. {errorMessage}");
				return;
			}

			if (_loadedCountMapByDataId.ContainsKey(audioDataId))
			{
				_loadedCountMapByDataId[audioDataId]++;
				return;
			}

			_loadedCountMapByDataId.Add(audioDataId, 1);

			var clipAddress = audioInfo.ClipAddress;
			Assert.IsTrue(HasValue(clipAddress), $"[AudioModule::LoadAssetAsync] ClipAddress is null by dataId: {audioDataId}.");

			var prefabAddress = HasValue(audioInfo.PrefabAddress) ? audioInfo.PrefabAddress : _config.DefaultPrefabAddress;
			Assert.IsTrue(HasValue(prefabAddress), $"[AudioModule::LoadAssetAsync] PrefabAddress is null by dataId: {audioDataId}.");

			var clip = await _clipAssetProvider.LoadAssetAsync<AudioClip>(clipAddress, cancellationToken);
			Assert.IsNotNull(clip, $"[AudioModule::LoadAssetAsync] Clip is not found by address: {clipAddress}.");

			_clipMapByAddress.TryAdd(clipAddress, clip);
			_loadedClipAddresses.Add(clipAddress);

			var prefab = await _prefabAssetProvider.LoadAssetAsync<GameObject>(prefabAddress, cancellationToken);
			_prefabMapByAddress.TryAdd(prefabAddress, prefab);
			_loadedPrefabAddresses.Add(clipAddress);
		}

		public async void UnloadAsset(string audioDataId)
		{
			if (!_audioInfoMapByDataId.TryGetValue(audioDataId, out var audioInfo))
			{
				Debug.LogError($"[AudioModule::UnloadAsset] AudioInfo is not find by dataId: {audioDataId}. Please check AudioModule config.");
				return;
			}

			if (!_loadedCountMapByDataId.ContainsKey(audioDataId))
				return;

			_loadedCountMapByDataId[audioDataId]--;
			if (_loadedCountMapByDataId[audioDataId] > 0)
				return;

			_loadedCountMapByDataId.Remove(audioDataId);

			await StopByDataIdAsync(audioDataId);

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

		public string Spawn(string audioDataId, string userId, float volume = 1, bool isLoop = false, Transform parent = null, Vector3 position = default, string callerId = null) =>
			Spawn(false, string.Empty, audioDataId, userId, volume, isLoop, parent, position, callerId);

		public string Spawn(bool isCustomId, string customId, string audioDataId, string userId, float volume = 1, bool isLoop = false, Transform parent = null, Vector3 position = default, string callerId = null)
		{
			if (!CheckIsAudioInfoLoaded(audioDataId, "Spawn", out var clipAddress, out var prefabAddress))
				return string.Empty;

			if (CheckIsOnCooldown(audioDataId))
				return string.Empty;

			AddCooldown(audioDataId);

			var audio = m_GetOrCreateAudio(prefabAddress);

			var audioId = isCustomId ? customId : Guid.NewGuid().ToString();
			var audioClip = _clipMapByAddress[clipAddress];

			AddAudioToPlayingAudiosMap(audioId, audio, parent, position);

			audio.GameObject.name = clipAddress;
			OnSpawn?.Invoke(callerId, audioId, audioDataId);

			audio.Play(userId, audioId, audioDataId, callerId, clipAddress, audioClip, volume, isLoop);
			return audio.Id;

			IAudio m_GetOrCreateAudio(string m_prefabAddress)
			{
				if (!_idleAudioMapByPrefabAddress.ContainsKey(m_prefabAddress))
					m_CreatePool(m_prefabAddress);

				var m_idleAudios = _idleAudioMapByPrefabAddress[m_prefabAddress];
				if (m_idleAudios.Count <= 0)
				{
					var m_prefab = _prefabMapByAddress[m_prefabAddress];
					var m_audio = Object.Instantiate(m_prefab).GetComponent<IAudio>();

					if (!TryGetAudioMixerGroup(out var audioMixerGroup))
						Debug.LogWarning($"[AudioModule::PlayAsync] AudioMixerGroup is not found by {_config.AudioMixerGroupPath}.");

					m_audio.Initialize(prefabAddress, audioMixerGroup);

					AddAudioToIdleAudiosMap(m_audio);
				}

				var m_idleAudio = m_idleAudios.First();
				m_idleAudios.Remove(m_idleAudio);
				return m_idleAudio;
			}

			void m_CreatePool(string m_prefabAddress)
			{
				_idleAudioMapByPrefabAddress.Add(m_prefabAddress, new List<IAudio>());

				var poolName = mm_GetPoolName(m_prefabAddress);
				var pool = new GameObject(poolName);
				var poolTransform = pool.transform;
				poolTransform.SetParent(_poolRoot);

				_poolMapByPrefabAddress.Add(m_prefabAddress, poolTransform);

				string mm_GetPoolName(string mm_prefabAddress) =>
					_config.PoolPrefix + mm_prefabAddress + _config.PoolSuffix;
			}
		}


		public UniTask<string> PlayAsync(string audioDataId, float volume = 1, float fadeTime = 0, bool isLoop = false, Transform parent = null, Vector3 position = default, string callerId = null) =>
			PlayAsync(string.Empty, audioDataId, volume, fadeTime, isLoop, parent, position, callerId);

		public async UniTask<string> PlayAsync(string audioDataId, string userId, float volume = 1, float fadeTime = 0, bool isLoop = false, Transform parent = null, Vector3 position = default, string callerId = null)
		{
			var audioId = Spawn(audioDataId, userId, volume, isLoop, parent, position, callerId);
			if (fadeTime > 0 && _playingAudioMapByAudioId.TryGetValue(audioId, out var playingAudio))
			{
				playingAudio.Resume();
				await AddTimer(audioId, 0, volume, fadeTime);
			}

			return audioId;
		}

		public async UniTask ModifyAsync(string audioId, float volume, bool isLoop, float fadeTime)
		{
			if (!IsInitialized)
				return;

			if (!_playingAudioMapByAudioId.TryGetValue(audioId, out var playingAudio))
			{
				Debug.LogWarning($"[AudioModule::ModifyAsync] Audio is not found by audioId: {audioId}.");
				return;
			}

			playingAudio.SetIsLoop(isLoop);
			await ModifyAsync(audioId, volume, fadeTime);
		}

		public async UniTask ModifyAsync(string audioId, float volume, float fadeTime)
		{
			if (!IsInitialized)
				return;

			if (!_playingAudioMapByAudioId.TryGetValue(audioId, out var playingAudio))
			{
				Debug.LogWarning($"[AudioModule::ModifyAsync] Audio is not found by audioId: {audioId}.");
				return;
			}

			if (fadeTime > 0)
				await AddTimer(audioId, playingAudio.Volume, volume, fadeTime);
			else
				playingAudio.SetVolume(volume);
		}

		public void SetTime(string audioId, float time)
		{
			if (!IsInitialized)
				return;

			if (!_playingAudioMapByAudioId.TryGetValue(audioId, out var playingAudio))
			{
				Debug.LogWarning($"[AudioModule::SetTime] Audio is not found by audioId: {audioId}.");
				return;
			}

			playingAudio.SetTime(time);
		}

		public void Resume(string audioId)
		{
			if (!IsInitialized)
				return;

			if (!_playingAudioMapByAudioId.TryGetValue(audioId, out var playingAudio))
			{
				Debug.LogWarning($"[AudioModule::Resume] Audio is not found by audioId: {audioId}.");
				return;
			}

			playingAudio.Resume();
		}

		public void Pause(string audioId)
		{
			if (!IsInitialized)
				return;

			if (!_playingAudioMapByAudioId.TryGetValue(audioId, out var playingAudio))
			{
				Debug.LogWarning($"[AudioModule::Pause] Audio is not found by audioId: {audioId}.");
				return;
			}

			playingAudio.Pause();
		}

		public void DeSpawn(string audioId) =>
			DeSpawn(audioId, null);

		public UniTask StopAsync(string audioId, float fadeTime = 0) =>
			StopAsync(audioId, fadeTime, null);

		public UniTask StopByDataIdAsync(string audioDataId, float fadeTime = 0)
		{
			if (!IsInitialized)
				return UniTask.CompletedTask;

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

		private async void DeSpawn(string audioId, Action<string, string, string> onComplete) =>
			await StopAsync(audioId, 0, onComplete);

		private async UniTask StopAsync(string audioId, float fadeTime, Action<string, string, string> onComplete)
		{
			if (!IsInitialized)
				return;

			if (string.IsNullOrEmpty(audioId) || string.IsNullOrWhiteSpace(audioId))
				return;

			if (!_playingAudioMapByAudioId.TryGetValue(audioId, out var playingAudio))
			{
				Debug.LogWarning($"[AudioModule::StopAsync] Audio is not found by audioId: {audioId}.");
				return;
			}

			if (fadeTime > 0)
				await AddTimer(audioId, playingAudio.Volume, 0, fadeTime);

			_playingAudioMapByAudioId.Remove(audioId);
			var callerId = playingAudio.CallerId;
			var audioDataId = playingAudio.DataId;
			playingAudio.Stop();
			AddAudioToIdleAudiosMap(playingAudio);

			OnStop?.Invoke(callerId, audioId, audioDataId);
			onComplete?.Invoke(callerId, audioId, audioDataId);
		}

		private bool CheckIsAudioInfoLoaded(string audioDataId, string methodName, out string clipAddress, out string prefabAddress)
		{
			if (!IsInitialized)
			{
				clipAddress = string.Empty;
				prefabAddress = string.Empty;
				return false;
			}

			if (!_loadedCountMapByDataId.ContainsKey(audioDataId))
			{
				clipAddress = string.Empty;
				prefabAddress = string.Empty;
				return false;
			}

			Assert.IsTrue(_audioInfoMapByDataId.ContainsKey(audioDataId), $"[AudioModule::{methodName}] AudioInfo is not find by dataId: {audioDataId}.");
			var audioInfo = _audioInfoMapByDataId[audioDataId];

			clipAddress = audioInfo.ClipAddress;
			if (!_clipMapByAddress.ContainsKey(clipAddress))
			{
				prefabAddress = string.Empty;
				return false;
			}

			prefabAddress = HasValue(audioInfo.PrefabAddress) ? audioInfo.PrefabAddress : _config.DefaultPrefabAddress;
			if (!_prefabMapByAddress.ContainsKey(prefabAddress))
				return false;


			return true;
		}

		private void AddAudioToIdleAudiosMap(IAudio audio)
		{
			audio.GameObject.name = audio.PrefabAddress;
			var pool = _poolMapByPrefabAddress[audio.PrefabAddress];
			SetAudioTransform(audio, false, pool, true, Vector3.zero);

			var idleAudios = _idleAudioMapByPrefabAddress[audio.PrefabAddress];
			idleAudios.Add(audio);
		}

		private void AddAudioToPlayingAudiosMap(string audioId, IAudio audio, Transform parent, Vector3 position)
		{
			var hasParent = parent != null;
			SetAudioTransform(audio, true, hasParent ? parent : _poolRoot, hasParent, position);
			_playingAudioMapByAudioId.Add(audioId, audio);
		}

		private void SetAudioTransform(IAudio audio, bool isActive, Transform parent, bool isLocalPosition, Vector3 position)
		{
			var audioGameObject = audio.GameObject;
			audioGameObject.SetActive(isActive);

			var audioTransform = audioGameObject.transform;

			audioTransform.SetParent(parent);
			if (isLocalPosition)
				audioTransform.localPosition = position;
			else
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

			var timerId = _timerModule.AddOnceTimer(startVolume, endVolume, duration, (_, value) => { audio.SetVolume(value); });
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


		private void AddCooldown(string audioDataId)
		{
			if (!_config.RestrictContinuousPlay.IsEnable)
				return;

			if (!_consecutiveCountMapByDataId.TryAdd(audioDataId, 1))
				_consecutiveCountMapByDataId[audioDataId]++;

			_timerModule.AddOnceTimer(_config.RestrictContinuousPlay.Duration, _ => RemoveCooldown(audioDataId));
		}

		private void RemoveCooldown(string audioDataId)
		{
			if (!_consecutiveCountMapByDataId.ContainsKey(audioDataId))
				return;

			_consecutiveCountMapByDataId[audioDataId]--;

			if (_consecutiveCountMapByDataId[audioDataId] <= 0)
				_consecutiveCountMapByDataId.Remove(audioDataId);
		}
	}
}