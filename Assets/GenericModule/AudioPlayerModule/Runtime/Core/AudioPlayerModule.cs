using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CizaAudioModule;
using CizaTimerModule;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

// namespace CizaAudioPlayerModule
// {
// 	public class AudioPlayerModule
// 	{
// 		//private variable
// 		private readonly AudioModule              _audioModule;
// 		private readonly IAudioPlayerModuleConfig _audioPlayerModuleConfig;
// 		private readonly TimerModule              _timerModule = new TimerModule();
//
// 		private readonly Dictionary<string, List<string>> _audioIdsMapByChannel = new Dictionary<string, List<string>>();
// 		private readonly Dictionary<string, float>        _volumeMapByAudioId   = new Dictionary<string, float>();
// 		private readonly Dictionary<string, string>       _timerIdMapByAudioId  = new Dictionary<string, string>();
//
// 		//public variable
// 		public AudioMixer AudioMixer    => _audioModule.AudioMixer;
// 		public bool       IsInitialized => _audioModule.IsInitialized;
// 		public bool       IsReleasing   { get; private set; }
//
// 		public string[] ClipDataIds   => _audioModule.ClipDataIds;
// 		public string[] PrefabDataIds => _audioModule.PrefabDataIds;
//
// 		//public method
// 		public AudioPlayerModule(IAudioPlayerModuleConfig audioPlayerModuleConfig, IAudioPlayerModuleAssetProvider audioPlayerModuleAssetProvider, AudioMixer audioMixer)
// 		{
// 			_audioPlayerModuleConfig = audioPlayerModuleConfig;
// 			_audioModule             = new AudioModule(_audioPlayerModuleConfig, audioPlayerModuleAssetProvider, audioMixer);
// 		}
//
// 		public void Initialize()
// 		{
// 			if (IsInitialized || IsReleasing)
// 				return;
//
// 			_timerModule.Initialize();
// 			_audioModule.Initialize();
// 		}
//
// 		public void Release()
// 		{
// 			if (!IsInitialized || IsReleasing)
// 				return;
//
// 			IsReleasing = true;
//
// 			StopAll(onComplete: () =>
// 			{
// 				_audioIdsMapByChannel.Clear();
// 				_audioModule.Release();
// 				_timerModule.Release();
//
// 				IsReleasing = false;
// 			});
// 		}
//
// 		public void Tick(float deltaTime) =>
// 			_timerModule.Tick(deltaTime);
//
// 		public bool CheckIsPlaying(string     audioId) => _audioModule.CheckIsPlaying(audioId);
// 		public void SetAudioMixerVolume(float volume) => _audioModule.SetVolume(volume);
//
// 		public bool CheckIsLoad(string clipDataId) => _audioModule.CheckIsLoad(clipDataId);
//
// 		public UniTask LoadAudio(string clipDataId, CancellationToken cancellationToken = default) => _audioModule.LoadAudioAssetAsync(clipDataId, cancellationToken);
//
// 		public bool TryPlay(string channel, string clipDataId, out string audioId, Vector3 position = default, Transform parentTransform = null, float volume = 1, bool isLocalPosition = false, bool isOverrideChannelPlaying = false) =>
// 			TryPlay(channel, clipDataId, out audioId, _audioPlayerModuleConfig.DefaultFadeTime, position, parentTransform, volume, isLocalPosition, isOverrideChannelPlaying);
//
// 		public bool TryPlay(string channel, string clipDataId, out string audioId, float fadeTime, Vector3 position = default, Transform parentTransform = null, float volume = 1, bool isLocalPosition = false, bool isOverrideChannelPlaying = false)
// 		{
// 			if (!_audioIdsMapByChannel.ContainsKey(channel))
// 				_audioIdsMapByChannel.Add(channel, new List<string>());
//
// 			var isPlay = _audioModule.TryPlay(clipDataId, out audioId, position, parentTransform, 0, isLocalPosition);
// 			if (!isPlay)
// 				return false;
//
// 			if (isOverrideChannelPlaying)
// 				StopByChannel(channel, fadeTime);
//
// 			FadeAudioVolume(audioId, 0, volume, fadeTime, null);
// 			_volumeMapByAudioId.Add(audioId, volume);
//
// 			var audioIds = _audioIdsMapByChannel[channel];
// 			audioIds.Add(audioId);
//
// 			return true;
// 		}
//
// 		public bool TryPlayAndAutoStop(string channel, string clipDataId, out string audioId, Vector3 position = default, Transform parentTransform = null, float volume = 1, Action onComplete = null, bool isOverrideChannelPlaying = false) =>
// 			TryPlayAndAutoStop(channel, clipDataId, out audioId, _audioPlayerModuleConfig.DefaultFadeTime, position, parentTransform, volume, onComplete, isOverrideChannelPlaying);
//
// 		public bool TryPlayAndAutoStop(string channel, string clipDataId, out string audioId, float fadeTime, Vector3 position = default, Transform parentTransform = null, float volume = 1, Action onComplete = null, bool isOverrideChannelPlaying = false)
// 		{
// 			var isPlay = TryPlay(channel, clipDataId, out audioId, fadeTime, position, parentTransform, volume, isOverrideChannelPlaying);
// 			if (!isPlay)
// 				return false;
//
// 			_audioModule.TryGetAudioReadModel(audioId, out var audioReadModel);
// 			var stoppedAudioId = audioId;
//
// 			var duration = audioReadModel.Duration;
// 			var timerId  = _timerModule.AddOnceTimer(duration, timerReadModel => { Stop(stoppedAudioId, 0, onComplete); });
// 			_timerIdMapByAudioId.Add(audioId, timerId);
//
// 			return true;
// 		}
//
// 		public void ChangeVolume(string audioId, float volume, Action onComplete = null) =>
// 			ChangeVolume(audioId, volume, _audioPlayerModuleConfig.DefaultFadeTime, onComplete);
//
// 		public void ChangeVolume(string audioId, float volume, float fadeTime, Action onComplete = null)
// 		{
// 			if (!_audioModule.TryGetAudioReadModel(audioId, out var audioReadModel))
// 			{
// 				Debug.LogWarning($"[AudioPlayerModule::ChangeVolume] AudioReadModel is not found by audioId: {audioId}.");
// 				return;
// 			}
//
// 			_volumeMapByAudioId[audioId] = volume;
// 			FadeAudioVolume(audioId, audioReadModel.Volume, volume, fadeTime, onComplete);
// 		}
//
// 		public void Pause(string audioId, Action onComplete = null) =>
// 			Pause(audioId, _audioPlayerModuleConfig.DefaultFadeTime, onComplete);
//
// 		public void Pause(string audioId, float fadeTime, Action onComplete = null)
// 		{
// 			if (!_audioModule.TryGetAudioReadModel(audioId, out var audioReadModel))
// 			{
// 				Debug.LogWarning($"[AudioPlayerModule::Pause] AudioReadModel is not found by audioId: {audioId}.");
// 				return;
// 			}
//
// 			var volume = audioReadModel.Volume;
// 			_volumeMapByAudioId[audioId] = volume;
//
// 			FadeAudioVolume(audioId, volume, 0, fadeTime, () =>
// 			{
// 				_audioModule.Pause(audioId);
// 				onComplete?.Invoke();
// 			});
// 		}
//
// 		public void PauseByChannel(string channel, Action onComplete = null) =>
// 			PauseByChannel(channel, _audioPlayerModuleConfig.DefaultFadeTime, onComplete);
//
// 		public void PauseByChannel(string channel, float fadeTime, Action onComplete = null)
// 		{
// 			if (_audioIdsMapByChannel.ContainsKey(channel))
// 			{
// 				Debug.LogWarning($"[AudioPlayerModule::PauseByChannel] Channel: {channel} is not found.");
// 				return;
// 			}
//
// 			var audioIds = _audioIdsMapByChannel[channel].ToArray();
// 			foreach (var audioId in audioIds)
// 				Pause(audioId, fadeTime);
//
// 			_timerModule.AddOnceTimer(fadeTime, timerReadModel => { onComplete?.Invoke(); });
// 		}
//
// 		public void Resume(string audioId, Action onComplete = null) =>
// 			Resume(audioId, _audioPlayerModuleConfig.DefaultFadeTime, onComplete);
//
// 		public void Resume(string audioId, float fadeTime, Action onComplete = null)
// 		{
// 			if (!IsInitialized)
// 			{
// 				Debug.LogWarning("[AudioPlayerModule::Resume] AudioPlayerModule is not initialized.");
// 				return;
// 			}
//
// 			if (!CheckIsPlaying(audioId))
// 			{
// 				Debug.LogWarning($"[AudioPlayerModule::Resume] Audio is not found by audioId: {audioId}.");
// 				return;
// 			}
//
// 			var volume = _volumeMapByAudioId[audioId];
// 			FadeAudioVolume(audioId, 0, volume, fadeTime, onComplete);
// 			_audioModule.Resume(audioId);
// 		}
//
// 		public void ResumeByChannel(string channel, Action onComplete = null) =>
// 			ResumeByChannel(channel, _audioPlayerModuleConfig.DefaultFadeTime, onComplete);
//
// 		public void ResumeByChannel(string channel, float fadeTime, Action onComplete = null)
// 		{
// 			var audioIds = _audioIdsMapByChannel[channel].ToArray();
// 			foreach (var audioId in audioIds)
// 				Resume(audioId, fadeTime);
//
// 			_timerModule.AddOnceTimer(fadeTime, timerReadModel => { onComplete?.Invoke(); });
// 		}
//
// 		public void Stop(string audioId, Action onComplete = null) =>
// 			Stop(audioId, _audioPlayerModuleConfig.DefaultFadeTime, onComplete);
//
// 		public void Stop(string audioId, float fadeTime, Action onComplete = null)
// 		{
// 			if (!_audioModule.TryGetAudioReadModel(audioId, out var audioReadModel))
// 			{
// 				Debug.LogWarning($"[AudioPlayerModule::Stop] AudioReadModel is not found by audioId: {audioId}.");
// 				return;
// 			}
//
// 			var volume = audioReadModel.Volume;
// 			_volumeMapByAudioId[audioId] = volume;
//
// 			FadeAudioVolume(audioId, volume, 0, fadeTime, () =>
// 			{
// 				if (CheckHasTimerId(audioId))
// 				{
// 					_timerIdMapByAudioId.Remove(audioId);
// 					_timerModule.RemoveTimer(audioId);
// 				}
//
// 				_audioModule.Stop(audioId);
//
// 				var channel  = GetChannelByAudioId(audioId);
// 				var audioIds = _audioIdsMapByChannel[channel];
// 				audioIds.Remove(audioId);
//
// 				onComplete?.Invoke();
// 			});
// 		}
//
// 		public void StopByChannel(string audioId, Action onComplete = null) =>
// 			StopByChannel(audioId, _audioPlayerModuleConfig.DefaultFadeTime, onComplete);
//
// 		public void StopByChannel(string channel, float fadeTime, Action onComplete = null)
// 		{
// 			Assert.IsTrue(_audioIdsMapByChannel.ContainsKey(channel),
// 			              $"[AudioPlayerModule::StopByChannel] Channel: {channel} doest exist.");
//
// 			var audioIds = _audioIdsMapByChannel[channel].ToArray();
//
// 			foreach (var audioId in audioIds)
// 				Stop(audioId, fadeTime);
//
// 			_timerModule.AddOnceTimer(fadeTime, timerReadModel => { onComplete?.Invoke(); });
// 		}
//
// 		public void StopAll(Action onComplete = null) =>
// 			StopAll(_audioPlayerModuleConfig.DefaultFadeTime, onComplete);
//
// 		public void StopAll(float fadeTime, Action onComplete = null)
// 		{
// 			var channels = _audioIdsMapByChannel.Keys.ToArray();
// 			foreach (var channel in channels)
// 				StopByChannel(channel, fadeTime);
//
// 			_timerModule.AddOnceTimer(fadeTime, timerReadModel => { onComplete?.Invoke(); });
// 		}
//
// 		//private method
// 		private string GetChannelByAudioId(string id)
// 		{
// 			var channels = _audioIdsMapByChannel.Keys.ToArray();
// 			foreach (var channel in channels)
// 			{
// 				var ids = _audioIdsMapByChannel[channel];
// 				foreach (var varId in ids)
// 				{
// 					if (varId == id)
// 						return channel;
// 				}
// 			}
//
// 			Debug.LogError($"[AudioPlayerModule::GetChannelById] Not find channel by id: {id}.");
// 			return string.Empty;
// 		}
//
// 		private void FadeAudioVolume(string audioId, float startVolume, float endVolume, float duration, Action onComplete) =>
// 			_timerModule.AddOnceTimer(startVolume, endVolume, duration, (timerReadModel, volume) => _audioModule.SetVolume(audioId, volume), timerReadModel => onComplete?.Invoke());
//
// 		private bool CheckHasTimerId(string id)
// 		{
// 			var hasTimerId = _timerIdMapByAudioId.ContainsKey(id);
// 			return hasTimerId;
// 		}
// 	}
// }
