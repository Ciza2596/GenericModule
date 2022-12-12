using System;
using System.Collections.Generic;
using AudioModule;


namespace AudioPlayerModule
{
    public class AudioPlayerModule
    {
        //private variable
        private const float DEFAULT_FADE_TIME = 0.25f;
        
        private readonly AudioModule.AudioModule _audioModule;
        private readonly Dictionary<string, string> _channelIdMaps = new Dictionary<string, string>();
        
        
        //public variable
        public bool IsReleasing { get; private set; }


        //public method
        public AudioPlayerModule(AudioModule.AudioModule audioModule) => _audioModule = audioModule;
        
        public void Initialize(List<IAudioData> audioDatas) => _audioModule.Initialize(audioDatas);

        public void Release()
        {
            IsReleasing = true;
            
            StopAll(onComplete: () =>
            {
                _channelIdMaps.Clear();
                _audioModule.Release();
                
                IsReleasing = false;
            });
        }
        
        
        //return id
        public string Play(string channel, string key, float fadeTime = DEFAULT_FADE_TIME, Action onComplete = null)
        {
            var id = _audioModule.Play(key);
            
            return id;
        }

        //changeVolume
        public void ChangeVolumeById(string id, float fadeTime = DEFAULT_FADE_TIME, Action onComplete = null)
        {
            
        }

        public void ChangeVolumeByChannel(string channel, float fadeTime = DEFAULT_FADE_TIME, Action onComplete = null)
        {
            
        }
        

        //stop
        public void StopById(string id, float fadeTime = DEFAULT_FADE_TIME, Action onComplete = null)
        {
            
        }

        public void StopByChannel(string channel, float fadeTime = DEFAULT_FADE_TIME, Action onComplete = null)
        {
        }

        public void StopAll(float fadeTime = DEFAULT_FADE_TIME, Action onComplete = null)
        {
            
        }
    }
}

