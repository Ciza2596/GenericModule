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

        private readonly string _masterVolumeParameter;
        private readonly string _bgmVolumeParameter;
        private readonly string _sfxVolumeParameter;
        private readonly string _voiceVolumeParameter;

        private readonly Transform _poolRootTransform;
        private readonly Dictionary<string, Transform> _keyPoolTransformMaps = new Dictionary<string, Transform>();

        private readonly Dictionary<string, List<string>> _keyIdsMaps = new Dictionary<string, List<string>>();
        private readonly List<string> _isPlayingIds = new List<string>();
        
        private readonly List<AudioData> _audioDatas = new List<AudioData>();


        private IAudioResourceData[] _audioResourceDatas;


        //public variable
        public AudioMixer AudioMixer { get; }


        //public method
        public AudioModule(IAudioModuleConfig audioModuleConfig)
        {
            Assert.IsNotNull(audioModuleConfig, "[AudioModule::AudioModule] AudioModuleConfig is null.");
            AudioMixer = audioModuleConfig.AudioMixer;

            _masterVolumeParameter = audioModuleConfig.MasterVolumeParameter;
            _bgmVolumeParameter = audioModuleConfig.BgmVolumeParameter;
            _sfxVolumeParameter = audioModuleConfig.SfxVolumeParameter;
            _voiceVolumeParameter = audioModuleConfig.VoiceVolumeParameter;


            var poolName = audioModuleConfig.PoolName;
            var poolGameObject = new GameObject(poolName);

            _poolRootTransform = poolGameObject.transform;
            var poolParentTransform = audioModuleConfig.PoolParentTransform;
            _poolRootTransform.SetParent(poolParentTransform);
        }

        public void Initialize(IAudioResourceData[] audioResourceDatas) => _audioResourceDatas = audioResourceDatas;

        public void Release()
        {
            _audioResourceDatas = null;

            _keyIdsMaps.Clear();
            _isPlayingIds.Clear();

            ReleasePool();
        }


        public void SetMasterVolume(float volume) =>
            AudioMixer.SetFloat(_masterVolumeParameter, volume);


        public void SetBgmVolume(float volume) =>
            AudioMixer.SetFloat(_bgmVolumeParameter, volume);


        public void SetSfxVolume(float volume) =>
            AudioMixer.SetFloat(_sfxVolumeParameter, volume);


        public void SetVoiceVolume(float volume) =>
            AudioMixer.SetFloat(_voiceVolumeParameter, volume);


        public string Play(string key, Vector3 position, Transform parentTransform)
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
            
            _isPlayingIds.Add(key);

            var audioData = GetAudioData(id);
            audioData.Play(position, parentTransform);

            return id;
        }

        
        public AudioData GetAudioData(string id) => _audioDatas.First(audioData => audioData.Id == id);


        public void Stop(string id)
        {
            if(!_isPlayingIds.Contains(id))
                return;

            var audioData = GetAudioData(id);
            audioData.Stop();
            
            _isPlayingIds.Remove(id);

            var key = audioData.Key;
            var keyIdsMap = _keyIdsMaps[key];
            keyIdsMap.Add(id);
        }


        //private method
        private void CreateAudioSource(string key)
        {
            var prefab = GetPrefab(key);
            Transform poolTransform;
            if (!_keyPoolTransformMaps.ContainsKey(key))
            {
                var poolGameObject = new GameObject(key);
                poolTransform = poolGameObject.transform;
                poolTransform.SetParent(_poolRootTransform);
                _keyPoolTransformMaps.Add(key, poolTransform);
            }

            poolTransform = _keyPoolTransformMaps[key];
            var gameObject = Object.Instantiate(prefab, poolTransform);
            if (gameObject.TryGetComponent<AudioSource>(out var audioSource))
            {
                var id = Guid.NewGuid().ToString();
                var audioData = new AudioData(id, key, audioSource, poolTransform);
                _audioDatas.Add(audioData);

                if (_keyIdsMaps.TryGetValue(key, out var ids))
                    ids.Add(id);

                else
                {
                    ids = new List<string> { id };
                    _keyIdsMaps.Add(key, ids);
                }
            }
            else
                Assert.IsNotNull(audioSource, $"[AudioModule::CreateAudioSource] Key: {key} prefab hasnt audioSource.");
        }


        private GameObject GetPrefab(string key)
        {
            Assert.IsNotNull(_audioResourceDatas, "[AudioModule::GetPrefab] AudioDatas is null.");

            var audioResourceData = _audioResourceDatas.First(data => data.Key == key);
            Assert.IsNotNull(audioResourceData, $"[AudioModule::GetPrefab] AudioData is null. Please check key: {key}.");

            var prefab = audioResourceData.Prefab;
            return prefab;
        }

        
        private void ReleasePool()
        {
            var audioDatas = _audioDatas.ToArray();
            _audioDatas.Clear();

            foreach (var audioData in audioDatas)
                audioData.Release();

            var poolTransforms = _keyPoolTransformMaps.Values.ToList();
            foreach (var poolTransform in poolTransforms)
                Object.Instantiate(poolTransform.gameObject);

            _keyPoolTransformMaps.Clear();
        }
    }
}