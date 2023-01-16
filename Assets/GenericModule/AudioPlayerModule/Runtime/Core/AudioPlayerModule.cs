using System;
using System.Collections.Generic;
using System.Linq;
using AudioModule;
using UnityEngine;
using UnityEngine.Assertions;


namespace AudioPlayerModule
{
    public class AudioPlayerModule
    {
        //private variable
        private const float DEFAULT_FADE_TIME = 0.25f;

        private readonly AudioModule.AudioModule _audioModule;
        private readonly ITween _tween;

        private readonly Dictionary<string, List<string>> _channelIdsMaps = new Dictionary<string, List<string>>();

        private readonly Dictionary<string, string> _audioIdTimerIdMaps = new Dictionary<string, string>();

        //public variable
        public bool IsReleasing { get; private set; }


        //public method
        public AudioPlayerModule(AudioModule.AudioModule audioModule, ITween tween)
        {
            _audioModule = audioModule;
            _tween = tween;
        }


        public void Initialize(IAudioResourceData[] audioResourceDatas) => _audioModule.Initialize(audioResourceDatas);

        public void Release()
        {
            IsReleasing = true;

            StopAll(onComplete: () =>
            {
                _channelIdsMaps.Clear();
                _audioModule.Release();

                IsReleasing = false;
            });
        }

        public void ReleasePool()
        {
            IsReleasing = true;

            StopAll(onComplete: () =>
            {
                _channelIdsMaps.Clear();
                _audioModule.ReleasePool();

                IsReleasing = false;
            });
        }


        public bool CheckIsPlaying(string id)
        {
            var isPlaying = _audioModule.CheckIsPlaying(id);
            return isPlaying;
        }

        public float GetOriginVolume(string id)
        {
            var audioData = _audioModule.GetAudioData(id);
            var originVolume = audioData.OriginVolume;
            return originVolume;
        }

        //play return id
        public string Play(string channel, string key, float fadeTime = DEFAULT_FADE_TIME, Transform parentTransform = null, bool isOverrideChannelPlaying = false) => Play(channel, key, fadeTime,
            Vector3.zero, parentTransform, isOverrideChannelPlaying);

        public string Play(string channel, string key, float fadeTime = DEFAULT_FADE_TIME,
            Vector3 position = default, Transform parentTransform = null,bool isOverrideChannelPlaying = false)
        {
            if (!_channelIdsMaps.ContainsKey(channel))
                _channelIdsMaps.Add(channel, new List<string>());

            if (isOverrideChannelPlaying)
                StopByChannel(channel, fadeTime);

            var ids = _channelIdsMaps[channel];
            var id = _audioModule.Play(key, position, parentTransform);

            var originVolume = GetOriginVolume(id);
            SetVolumeByFade(id, 0, originVolume, fadeTime);

            ids.Add(id);

            return id;
        }

        public string PlayAndAutoStop(string channel, string key, float fadeTime = DEFAULT_FADE_TIME,
            Vector3 position = default, Transform parentTransform = null,
            Action onComplete = null, bool isOverrideChannelPlaying = false)
        {
            var id = Play(channel, key, fadeTime, position, parentTransform, isOverrideChannelPlaying);
            var audioData = _audioModule.GetAudioData(id);

            var duration = audioData.Duration;
            var timerId = _tween.PlayTimer(duration, () =>
            {
                if (CheckIsPlaying(id))
                    Stop(id, 0, onComplete);
            });
            _audioIdTimerIdMaps.Add(id, timerId);

            return id;
        }


        //changeVolume
        public void ChangeVolume(string id, float volume, float fadeTime = DEFAULT_FADE_TIME)
        {
            var audioData = _audioModule.GetAudioData(id);
            SetVolumeByFade(id, audioData.Volume, volume, fadeTime);
        }

        public void ChangeVolumeByChannel(string channel, float volume, float fadeTime = DEFAULT_FADE_TIME)
        {
            var ids = _channelIdsMaps[channel];

            foreach (var id in ids)
                ChangeVolume(id, volume, fadeTime);
        }


        //stop
        public void Stop(string id, float fadeTime = DEFAULT_FADE_TIME, Action onComplete = null)
        {
            var channel = GetChannelById(id);
            var ids = _channelIdsMaps[channel];

            var audioData = _audioModule.GetAudioData(id);

            _tween.To(audioData.Volume, volume => audioData.SetVolume(volume), 0, fadeTime, () =>
            {
                if (CheckHasTimerId(id))
                {
                    _audioIdTimerIdMaps.Remove(id);
                    _tween.StopTimer(id);
                }

                _audioModule.Stop(id);
                ids.Remove(id);

                onComplete?.Invoke();
            });
        }

        public void StopByChannel(string channel, float fadeTime = DEFAULT_FADE_TIME, Action onComplete = null)
        {
            Assert.IsTrue(_channelIdsMaps.ContainsKey(channel), $"[AudioPlayerModule::StopByChannel] Channel: {channel} doest exist.");
            
            var ids = _channelIdsMaps[channel].ToArray();
            foreach (var id in ids)
                Stop(id, fadeTime);

            _tween.PlayTimer(fadeTime, onComplete);
        }

        public void StopAll(float fadeTime = DEFAULT_FADE_TIME, Action onComplete = null)
        {
            var channels = _channelIdsMaps.Keys.ToArray();
            foreach (var channel in channels)
                StopByChannel(channel, fadeTime);

            _tween.PlayTimer(fadeTime, onComplete);
        }


        //private
        private string GetChannelById(string id)
        {
            var channels = _channelIdsMaps.Keys.ToArray();
            foreach (var channel in channels)
            {
                var ids = _channelIdsMaps[channel];
                foreach (var varId in ids)
                {
                    if (varId == id)
                        return channel;
                }
            }

            Debug.LogError($"[AudioPlayerModule::GetChannelById] Not find channel by id: {id}.");
            return string.Empty;
        }

        private void SetVolumeByFade(string id, float startVolume, float endVolume, float durationTime,
            Action onComplete = null)
        {
            var audioData = _audioModule.GetAudioData(id);

            _tween.To(startVolume, volume => audioData.SetVolume(volume), endVolume, durationTime, onComplete);
        }

        private bool CheckHasTimerId(string id)
        {
            var hasTimerId = _audioIdTimerIdMaps.ContainsKey(id);
            return hasTimerId;
        }
    }
}