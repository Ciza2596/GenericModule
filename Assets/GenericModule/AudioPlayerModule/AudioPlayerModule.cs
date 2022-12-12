using System;
using System.Collections.Generic;
using System.Linq;
using AudioModule;
using UnityEngine;


namespace AudioPlayerModule
{
    public class AudioPlayerModule
    {
        //private variable
        private const float DEFAULT_FADE_TIME = 0.25f;

        private readonly AudioModule.AudioModule _audioModule;
        private readonly Dictionary<string, List<string>> _channelIdsMaps = new Dictionary<string, List<string>>();


        //public variable
        public bool IsReleasing { get; private set; }


        //public method
        public AudioPlayerModule(AudioModule.AudioModule audioModule) => _audioModule = audioModule;


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


        //return id
        public string Play(string channel, string key, float fadeTime = DEFAULT_FADE_TIME, bool isOverride = false,
            Vector3 position = default, Transform parentTransform = null)
        {
            if (isOverride)
                StopByChannel(channel, fadeTime);

            if (!_channelIdsMaps.ContainsKey(channel))
                _channelIdsMaps.Add(channel, new List<string>());

            var ids = _channelIdsMaps[channel];
            var id = _audioModule.Play(key, position, parentTransform);
            ids.Add(id);

            return id;
        }

        public string PlayAndAutoStop(string channel, string key, float fadeTime = DEFAULT_FADE_TIME,
            bool isOverride = false, Vector3 position = default, Transform parentTransform = null,
            Action onComplete = null)
        {
            var id = _audioModule.Play(key, position, parentTransform);

            return id;
        }


        //changeVolume
        public void ChangeVolume(string id, float volume, float fadeTime = DEFAULT_FADE_TIME)
        {
            var audioData = _audioModule.GetAudioData(id);
            audioData.SetVolume(volume);
        }

        public void ChangeVolumeByChannel(string channel, float volume, float fadeTime = DEFAULT_FADE_TIME)
        {
            var ids = _channelIdsMaps[channel];

            foreach (var id in ids)
                ChangeVolume(id, volume, fadeTime);
        }

        public void ResetVolume(string id, float volume, float fadeTime = DEFAULT_FADE_TIME)
        {
            var audioData = _audioModule.GetAudioData(id);
            var originVolume = audioData.OriginVolume;
            audioData.SetVolume(originVolume);
        }
        
        public void ResetVolumeByChannel(string channel, float volume, float fadeTime = DEFAULT_FADE_TIME)
        {
            var ids = _channelIdsMaps[channel];

            foreach (var id in ids)
                ResetVolume(id, volume, fadeTime);
        }


        //stop
        public void StopById(string id, float fadeTime = DEFAULT_FADE_TIME, Action onComplete = null)
        {
            var channel = GetChannelById(id);
            var ids = _channelIdsMaps[channel];

            _audioModule.Stop(id);
            ids.Remove(id);
        }

        public void StopByChannel(string channel, float fadeTime = DEFAULT_FADE_TIME, Action onComplete = null)
        {
            var ids = _channelIdsMaps[channel].ToArray();
            foreach (var id in ids)
                StopById(id, fadeTime, onComplete);
        }

        public void StopAll(float fadeTime = DEFAULT_FADE_TIME, Action onComplete = null)
        {
            var channels = _channelIdsMaps.Keys.ToArray();
            foreach (var channel in channels)
                StopByChannel(channel, fadeTime, onComplete);
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

            Debug.LogError("[AudioPlayerModule::GetChannelById] Not find channel by ");
            return null;
        }
    }
}