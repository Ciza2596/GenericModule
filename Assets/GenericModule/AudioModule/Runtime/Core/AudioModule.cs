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
        private readonly string _poolPrefix;
        private readonly string _poolSuffix;
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


            var poolRootName = audioModuleConfig.PoolRootName;
            var poolRootGameObject = new GameObject(poolRootName);
            _poolRootTransform = poolRootGameObject.transform;


            _poolPrefix = audioModuleConfig.PoolPrefix;
            _poolSuffix = audioModuleConfig.PoolSuffix;
        }

        public void Initialize(IAudioResourceData[] audioResourceDatas) => _audioResourceDatas = audioResourceDatas;

        public void Release()
        {
            _audioResourceDatas = null;

            ReleaseAllPools();
        }

        public void ReleaseAllPools()
        {
            _keyIdsMaps.Clear();
            _isPlayingIds.Clear();

            var audioDatas = _audioDatas.ToArray();
            _audioDatas.Clear();

            foreach (var audioData in audioDatas)
                audioData.Release();

            var poolTransforms = _keyPoolTransformMaps.Values.ToList();
            foreach (var poolTransform in poolTransforms)
                Object.Destroy(poolTransform.gameObject);

            _keyPoolTransformMaps.Clear();
        }

        public void ReleasePool(string key)
        {
            var ids = _keyIdsMaps[key];
            _keyIdsMaps.Remove(key);

            foreach (var id in ids)
                if (_isPlayingIds.Contains(id))
                    _isPlayingIds.Remove(id);

            ids.Clear();

            var audioDatas = _audioDatas.Where(audioData => audioData.Key == key).ToArray();

            foreach (var audioData in audioDatas)
            {
                _audioDatas.Remove(audioData);
                audioData.Release();
            }

            var poolTransform = _keyPoolTransformMaps[key];
            _keyPoolTransformMaps.Remove(key);
            Object.Destroy(poolTransform.gameObject);
        }


        public void SetMasterVolume(float volume) =>
            SetAudioMixerFloat(_masterVolumeParameter, volume);


        public void SetBgmVolume(float volume) =>
            SetAudioMixerFloat(_bgmVolumeParameter, volume);


        public void SetSfxVolume(float volume) => 
            SetAudioMixerFloat(_sfxVolumeParameter, volume);


        public void SetVoiceVolume(float volume) =>
            SetAudioMixerFloat(_voiceVolumeParameter, volume);


        public bool CheckIsPlaying(string id) =>
            _isPlayingIds.Contains(id);

        public AudioData GetAudioData(string id)
        {
            var audioData = _audioDatas.Find(audioData => audioData.Id == id);
            Assert.IsNotNull(audioData, $"[AudioModule::GetAudioData] Not find audioData by id: {id}.");
            return audioData;
        }

        public string Play(string key, Transform parentTransform = null) => Play(key, Vector3.zero, parentTransform);

        public string Play(string key, Vector3 localPosition, Transform parentTransform = null)
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

            _isPlayingIds.Add(id);

            var audioData = GetAudioData(id);
            audioData.Play(localPosition, parentTransform);

            return id;
        }


        public void Resume(string id)
        {
            if (!CheckIsPlaying(id))
            {
                Debug.Log("[AudioModule::Resume] Not find playing audio.");
                return;
            }

            var audioData = GetAudioData(id);
            audioData.Resume();
        }

        public void Pause(string id)
        {
            if (!CheckIsPlaying(id))
            {
                Debug.Log("[AudioModule::Pause] Not find playing audio.");
                return;
            }

            var audioData = GetAudioData(id);
            audioData.Pause();
        }

        public void Stop(string id)
        {
            if (!CheckIsPlaying(id))
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
                var poolGameObject = new GameObject(_poolPrefix + key + _poolSuffix);
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
            Assert.IsNotNull(audioResourceData,
                $"[AudioModule::GetPrefab] AudioData is null. Please check key: {key}.");

            var prefab = audioResourceData.Prefab;
            return prefab;
        }


        private void SetAudioMixerFloat(string volumeParameter, float volume)
        {
            var linearToLogarithmicScale = GetLinearToLogarithmicScale(volume);
            AudioMixer.SetFloat(volumeParameter, linearToLogarithmicScale);
        }

        private float GetLinearToLogarithmicScale(float value) =>
            Mathf.Log(Mathf.Clamp(value, 0.001f, 1)) * 20.0f;
    }
}