using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace AudioModule
{
    public class AudioModule
    {
        //private variable
        private IAudioModuleConfig _audioModuleConfig;
        private List<IAudioData> _audioDatas;
        

        private Transform _poolTransform;
        
        
        private Dictionary<string, List<string>> _keyIdsMaps = new Dictionary<string, List<string>>();
        private Dictionary<string, string> _playingIdKeyMaps = new Dictionary<string, string>();
        
        
        private Dictionary<string, AudioSource> _pool = new Dictionary<string, AudioSource>();
        

        //public variable
        public AudioMixer AudioMixer => _audioModuleConfig.AudioMixer;


        //public method
        public AudioModule(IAudioModuleConfig audioModuleConfig)
        {
            Assert.IsNotNull(audioModuleConfig, "[AudioModule::AudioModule] AudioModuleConfig is null.");
            _audioModuleConfig = audioModuleConfig;

            var poolGameObject = new GameObject(_audioModuleConfig.PoolName);
            _poolTransform = poolGameObject.transform;
            _poolTransform.SetParent(_audioModuleConfig.PoolParentTransform);
        }

        public void Initialize(List<IAudioData> audioDatas) => _audioDatas = audioDatas;

        public void Release()
        {
            _audioDatas = null;
            
            _keyIdsMaps.Clear();
            _playingIdKeyMaps.Clear();
            
            ReleasePool();
        }


        public void SetMasterVolume(float volume)
        {
            var volumeParameter = _audioModuleConfig.MasterVolumeParameter;
            AudioMixer.SetFloat(volumeParameter, volume);
        }

        public void SetBgmVolume(float volume)
        {
            var volumeParameter = _audioModuleConfig.BgmVolumeParameter;
            AudioMixer.SetFloat(volumeParameter, volume);
        }

        public void SetSfxVolume(float volume)
        {
            var volumeParameter = _audioModuleConfig.SfxVolumeParameter;
            AudioMixer.SetFloat(volumeParameter, volume);
        }

        public void SetVoiceVolume(float volume)
        {
            var volumeParameter = _audioModuleConfig.VoiceVolumeParameter;
            AudioMixer.SetFloat(volumeParameter, volume);
        }


        public string Play(string key)
        {
            if (!_keyIdsMaps.ContainsKey(key))
            {
                var newIds = new List<string>();
                _keyIdsMaps.Add(key, newIds);
            }

            var ids = _keyIdsMaps[key];

            if (ids.Count <= 0)
                CreateAudioSource(key);
            
            var id = ids[0];
            ids.Remove(id);
            
            _playingIdKeyMaps.Add(id, key);

            Play(id, key);

            return id;
        }


        public void Stop(string id)
        {
            var audioSource = _pool[id];
            audioSource.Stop();

            var key = _playingIdKeyMaps[id];
            _playingIdKeyMaps.Remove(id);

            var ids = _keyIdsMaps[key];
            ids.Add(id);
        }


        //private method
        private void CreateAudioSource(string key)
        {
            var prefab = GetPrefab(key);
            var gameObject = Object.Instantiate(prefab,_poolTransform);
            if (gameObject.TryGetComponent<AudioSource>(out var audioSource))
            {
                var id = Guid.NewGuid().ToString();
                _pool.Add(id, audioSource);

                if (_keyIdsMaps.TryGetValue(key, out var ids))
                    ids.Add(id);
                
                else
                {
                    ids = new List<string> { id };
                    _keyIdsMaps.Add(key, ids);
                }
            }
            else
                Assert.IsNotNull(audioSource, "[AudioModule::CreateAudioSource] ");
        }

        private void Play(string id, string key)
        {
            var audioSource = _pool[id];
            
            Assert.IsNotNull(audioSource.clip,$"[AudioModule::Play] Clip is null. Please check Key: {key} audioPrefab.");
            audioSource.Play();
        }


        private GameObject GetPrefab(string key)
        {
            Assert.IsNotNull(_audioDatas, "[AudioModule::GetPrefab] AudioDatas is null.");

            var audioData = _audioDatas.Find(data => data.Key == key);
            Assert.IsNotNull(audioData, $"[AudioModule::GetPrefab] AudioData is null. Please check key: {key}.");

            var prefab = audioData.Prefab;
            return prefab;
        }

        private void ReleasePool()
        {
            var pool = _pool.Values.ToArray();
            _pool.Clear();

            foreach (var audioSource in pool)
                Object.Destroy(audioSource.gameObject);
        }
    }
}