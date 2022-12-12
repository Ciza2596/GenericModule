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

        private readonly Transform _poolTransform;

        private readonly Dictionary<string, List<string>> _keyIdsMaps = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, string> _playingIdKeyMaps = new Dictionary<string, string>();

        private readonly Dictionary<string, AudioSource> _pool = new Dictionary<string, AudioSource>();


        private List<IAudioData> _audioDatas;


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

            _poolTransform = poolGameObject.transform;
            var poolParentTransform = audioModuleConfig.PoolParentTransform;
            _poolTransform.SetParent(poolParentTransform);
        }

        public void Initialize(List<IAudioData> audioDatas) => _audioDatas = audioDatas;

        public void Release()
        {
            _audioDatas = null;

            _keyIdsMaps.Clear();
            _playingIdKeyMaps.Clear();

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

        public AudioSource GetAudioSource(string id)
        {
            var audioSource = _pool[id];
            return audioSource;
        }

        public void Stop(string id)
        {
            var audioSource = _pool[id];
            audioSource.Stop();

            if (_playingIdKeyMaps.TryGetValue(id, out var key))
            {
                _playingIdKeyMaps.Remove(id);

                var ids = _keyIdsMaps[key];
                if (!ids.Contains(id))
                    ids.Add(id);
            }
        }


        //private method
        private void CreateAudioSource(string key)
        {
            var prefab = GetPrefab(key);
            var gameObject = Object.Instantiate(prefab, _poolTransform);
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

            Assert.IsNotNull(audioSource.clip,
                $"[AudioModule::Play] Clip is null. Please check Key: {key} audioPrefab.");
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