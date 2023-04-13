using System;
using System.Collections.Generic;
using System.Linq;
using CizaAudioModule;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;


namespace CizaAudioPlayerModule
{
    public class AudioPlayerModule
    {
        //private variable
        private readonly AudioModule _audioModule;
        private readonly IAudioPlayerModuleConfig _audioPlayerModuleConfig;
        private readonly ITimerModule _timerModule;

        private readonly Dictionary<string, List<string>> _audioIdsMapByChannel = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, float> _volumeMapByAudioId = new Dictionary<string, float>();
        private readonly Dictionary<string, string> _timerIdMapByAudioId = new Dictionary<string, string>();
        private readonly List<string> _genericTimerIds = new List<string>();
        

        //public variable
        public AudioMixer AudioMixer => _audioModule.AudioMixer;
        public bool IsInitialized => _audioModule.IsInitialized;

        public string[] ClipDataIds => _audioModule.ClipDataIds;
        public string[] PrefabDataIds => _audioModule.PrefabDataIds;
        public string[] AssetDataIds => _audioModule.AssetDataIds;

        public bool IsReleasing { get; private set; }


        //public method
        public AudioPlayerModule(IAudioPlayerModuleConfig audioPlayerModuleConfig, IAudioPlayerModuleAssetProvider audioPlayerModuleAssetProvider, ITimerModule timerModule, AudioMixer audioMixer)
        {
            _audioPlayerModuleConfig = audioPlayerModuleConfig;
            _audioModule = new AudioModule(_audioPlayerModuleConfig, audioPlayerModuleAssetProvider, audioMixer);

            _timerModule = timerModule;
        }

        public UniTask Initialize(Dictionary<string, IAudioData> audioDatasMap) => _audioModule.Initialize(audioDatasMap);

        public void Release()
        {
            IsReleasing = true;

            StopAll(onComplete: () =>
            {
                _audioIdsMapByChannel.Clear();
                _audioModule.Release();

                IsReleasing = false;
            });
        }

        public bool CheckIsPlaying(string audioId) => _audioModule.CheckIsPlaying(audioId);
        public void SetAudioMixerVolume(float volume) => _audioModule.SetAudioMixerVolume(volume);
        
        public string Play(string channel, string clipDataId, Vector3 position = default, Transform parentTransform = null, float volume = 1, bool isLocalPosition = false, bool isOverrideChannelPlaying = false) =>
            Play(channel, clipDataId, _audioPlayerModuleConfig.DefaultFadeTime, position, parentTransform, volume, isLocalPosition, isOverrideChannelPlaying);
        public string Play(string channel, string clipDataId, float fadeTime, Vector3 position = default, Transform parentTransform = null, float volume = 1, bool isLocalPosition = false, bool isOverrideChannelPlaying = false)
        {
            if (!_audioIdsMapByChannel.ContainsKey(channel))
                _audioIdsMapByChannel.Add(channel, new List<string>());

            if (isOverrideChannelPlaying)
                StopByChannel(channel, fadeTime);

            var audioId = _audioModule.Play(clipDataId, position, parentTransform, 0, isLocalPosition);
            FadeAudioVolume(audioId, 0, volume, fadeTime, null);
            _volumeMapByAudioId.Add(audioId, volume);

            var audioIds = _audioIdsMapByChannel[channel];
            audioIds.Add(audioId);

            return audioId;
        }
        
        public string PlayAndAutoStop(string channel, string clipDataId, Vector3 position = default, Transform parentTransform = null, float volume = 1, Action onComplete = null, bool isOverrideChannelPlaying = false) =>
            PlayAndAutoStop(channel, clipDataId, _audioPlayerModuleConfig.DefaultFadeTime, position, parentTransform, volume, onComplete, isOverrideChannelPlaying);
        public string PlayAndAutoStop(string channel, string clipDataId, float fadeTime, Vector3 position = default, Transform parentTransform = null, float volume = 1, Action onComplete = null, bool isOverrideChannelPlaying = false)
        {
            var audioId = Play(channel, clipDataId, fadeTime, position, parentTransform, volume, isOverrideChannelPlaying);
            _audioModule.TryGetAudioReadModel(audioId, out var audioReadModel);

            var duration = audioReadModel.Duration;
            var timerId = _timerModule.AddOnceTimer(duration, () => { Stop(audioId, 0, onComplete); });
            _timerIdMapByAudioId.Add(audioId, timerId);

            return audioId;
        }

        public void ChangeVolume(string audioId, float volume, Action onComplete = null) =>
            ChangeVolume(audioId, volume, _audioPlayerModuleConfig.DefaultFadeTime, onComplete);
        public void ChangeVolume(string audioId, float volume, float fadeTime, Action onComplete = null)
        {
            if (!_audioModule.TryGetAudioReadModel(audioId, out var audioReadModel))
            {
                Debug.LogWarning($"[AudioPlayerModule::ChangeVolume] AudioReadModel is not found by audioId: {audioId}.");
                return;
            }

            _volumeMapByAudioId[audioId] = volume;
            FadeAudioVolume(audioId, audioReadModel.Volume, volume, fadeTime, onComplete);
        }


        public void Pause(string audioId, Action onComplete = null) =>
            Pause(audioId, _audioPlayerModuleConfig.DefaultFadeTime, onComplete);
        public void Pause(string audioId, float fadeTime, Action onComplete = null)
        {
            if (!_audioModule.TryGetAudioReadModel(audioId, out var audioReadModel))
            {
                Debug.LogWarning($"[AudioPlayerModule::Pause] AudioReadModel is not found by audioId: {audioId}.");
                return;
            }

            var volume = audioReadModel.Volume;
            _volumeMapByAudioId[audioId] = volume;

            FadeAudioVolume(audioId, volume, 0, fadeTime, () =>
            {
                _audioModule.Pause(audioId);
                onComplete?.Invoke();
            });
        }

        public void PauseByChannel(string channel, Action onComplete = null) =>
            PauseByChannel(channel, _audioPlayerModuleConfig.DefaultFadeTime, onComplete);
        public void PauseByChannel(string channel, float fadeTime, Action onComplete = null)
        {
            if (_audioIdsMapByChannel.ContainsKey(channel))
            {
                Debug.LogWarning($"[AudioPlayerModule::PauseByChannel] Channel: {channel} is not found.");
                return;
            }

            var audioIds = _audioIdsMapByChannel[channel].ToArray();
            foreach (var audioId in audioIds)
                Pause(audioId, fadeTime);

            _timerModule.AddOnceTimer(fadeTime, onComplete);
        }


        public void Resume(string audioId, Action onComplete = null) =>
            Resume(audioId, _audioPlayerModuleConfig.DefaultFadeTime, onComplete);
        public void Resume(string audioId, float fadeTime, Action onComplete = null)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[AudioPlayerModule::Resume] AudioPlayerModule is not initialized.");
                return;
            }
            
            if (!CheckIsPlaying(audioId))
            {
                Debug.LogWarning($"[AudioPlayerModule::Resume] Audio is not found by audioId: {audioId}.");
                return;
            }
            
            var volume = _volumeMapByAudioId[audioId];
            FadeAudioVolume(audioId,0, volume, fadeTime, onComplete);
            _audioModule.Resume(audioId);
        }

        
        public void ResumeByChannel(string channel,  Action onComplete = null) =>
            ResumeByChannel(channel, _audioPlayerModuleConfig.DefaultFadeTime, onComplete);
        public void ResumeByChannel(string channel, float fadeTime, Action onComplete = null)
        {
            var audioIds = _audioIdsMapByChannel[channel].ToArray();
            foreach (var audioId in audioIds)
                Resume(audioId, fadeTime);

            _timerModule.AddOnceTimer(fadeTime, onComplete);
        }


        public void Stop(string audioId, Action onComplete = null) =>
            Stop(audioId, _audioPlayerModuleConfig.DefaultFadeTime, onComplete);
        public void Stop(string audioId, float fadeTime, Action onComplete = null)
        {
            if (!_audioModule.TryGetAudioReadModel(audioId, out var audioReadModel))
            {
                Debug.LogWarning($"[AudioPlayerModule::Stop] AudioReadModel is not found by audioId: {audioId}.");
                return;
            }

            var volume = audioReadModel.Volume;
            _volumeMapByAudioId[audioId] = volume;

            FadeAudioVolume(audioId, volume, 0, fadeTime, () =>
            {
                if (CheckHasTimerId(audioId))
                {
                    _timerIdMapByAudioId.Remove(audioId);
                    _timerModule.StopTimer(audioId);
                }

                _audioModule.Stop(audioId);
                
                var channel = GetChannelByAudioId(audioId);
                var audioIds = _audioIdsMapByChannel[channel];
                audioIds.Remove(audioId);

                onComplete?.Invoke();
            });
        }

        public void StopByChannel(string audioId, Action onComplete = null) =>
            StopByChannel(audioId, _audioPlayerModuleConfig.DefaultFadeTime, onComplete);
        public void StopByChannel(string channel, float fadeTime, Action onComplete = null)
        {
            Assert.IsTrue(_audioIdsMapByChannel.ContainsKey(channel),
                $"[AudioPlayerModule::StopByChannel] Channel: {channel} doest exist.");

            var audioIds = _audioIdsMapByChannel[channel].ToArray();
            
            foreach (var audioId in audioIds)
                Stop(audioId, fadeTime);

            _timerModule.AddOnceTimer(fadeTime, onComplete);
        }

        public void StopAll(Action onComplete = null) =>
            StopAll(_audioPlayerModuleConfig.DefaultFadeTime, onComplete);
        public void StopAll(float fadeTime, Action onComplete = null)
        {
            var channels = _audioIdsMapByChannel.Keys.ToArray();
            foreach (var channel in channels)
                StopByChannel(channel, fadeTime);

            _timerModule.AddOnceTimer(fadeTime, onComplete);
        }


        //private method
        private string GetChannelByAudioId(string id)
        {
            var channels = _audioIdsMapByChannel.Keys.ToArray();
            foreach (var channel in channels)
            {
                var ids = _audioIdsMapByChannel[channel];
                foreach (var varId in ids)
                {
                    if (varId == id)
                        return channel;
                }
            }

            Debug.LogError($"[AudioPlayerModule::GetChannelById] Not find channel by id: {id}.");
            return string.Empty;
        }

        private void FadeAudioVolume(string audioId, float startVolume, float endVolume, float duration, Action onComplete) =>
            _timerModule.AddOnceTimer(startVolume, volume => _audioModule.SetVolume(audioId, volume), endVolume, duration, onComplete);
        
        private bool CheckHasTimerId(string id)
        {
            var hasTimerId = _timerIdMapByAudioId.ContainsKey(id);
            return hasTimerId;
        }
    }
}