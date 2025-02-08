using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using UnityEngine.Scripting;

namespace CizaAudioModule.Implement
{
    [CreateAssetMenu(fileName = "AudioPlayerConfig", menuName = "Ciza/AudioModule/AudioPlayerConfig", order = 100)]
    public class AudioPlayerConfig : ScriptableObject, IAudioPlayerConfig
    {
        [SerializeField]
        private string _rootName = "[AudioPlayer]";

        [SerializeField]
        private bool _isDontDestroyOnLoad = true;

        [Space]
        [SerializeField]
        private AudioMixer _audioMixer;

        [SerializeField]
        private string _masterMixerGroupPath = "Master";

        [SerializeField]
        private string _masterMixerParameter = "Master";

        [Range(0, 1)]
        [SerializeField]
        private float _defaultMasterVolume = 0.7f;

        [Space]
        [SerializeField]
        private AudioModuleConfig _bgmModuleConfig = new AudioModuleConfig("Bgm", "Master/Bgm", "Bgm", "BgmAudio");

        [SerializeField]
        private AudioModuleConfig _sfxModuleConfig = new AudioModuleConfig("Sfx", "Master/Sfx", "Sfx", "SfxAudio");

        [SerializeField]
        private AudioModuleConfig _voiceModuleConfig = new AudioModuleConfig("Voice", "Master/Voice", "Voice", "VoiceAudio");

        public string RootName => _rootName;
        public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

        public AudioMixer AudioMixer => _audioMixer;
        public string MasterMixerGroupPath => _masterMixerGroupPath;
        public string MasterMixerParameter => _masterMixerParameter;
        public float DefaultMasterVolume => _defaultMasterVolume;

        public IAudioModuleConfig BgmModuleConfig => _bgmModuleConfig;
        public IAudioModuleConfig SfxModuleConfig => _sfxModuleConfig;
        public IAudioModuleConfig VoiceModuleConfig => _voiceModuleConfig;

        [Serializable]
        private class AudioModuleConfig : IAudioModuleConfig
        {
            [SerializeField]
            private string _poolRootName;

            [Space]
            [SerializeField]
            private string _poolPrefix;

            [SerializeField]
            private string _poolSuffix = "s";

            [Space]
            [SerializeField]
            private string _audioMixerGroupPath;

            [SerializeField]
            private string _audioMixerParameter;

            [Range(0, 1)]
            [SerializeField]
            private float _defaultVolume = 0.7f;

            [Space]
            [SerializeField]
            private RestrictContinuousPlayImp _restrictContinuousPlay;

            [Space]
            [SerializeField]
            private string _defaultPrefabAddress;

            [SerializeField]
            private AudioInfo[] _audioInfos;

            [Preserve]
            public AudioModuleConfig() { }

            [Preserve]
            public AudioModuleConfig(string poolRootName, string audioMixerGroupPath, string audioMixerParameter, string defaultPrefabAddress)
            {
                _poolRootName = poolRootName;
                _audioMixerGroupPath = audioMixerGroupPath;
                _audioMixerParameter = audioMixerParameter;
                _defaultPrefabAddress = defaultPrefabAddress;
            }

            public string PoolRootName => _poolRootName;
            public float DefaultVolume => _defaultVolume;

            public IRestrictContinuousPlay RestrictContinuousPlay => _restrictContinuousPlay;

            public string PoolPrefix => _poolPrefix;
            public string PoolSuffix => _poolSuffix;

            public string AudioMixerGroupPath => _audioMixerGroupPath;
            public string AudioMixerParameter => _audioMixerParameter;
            public string DefaultPrefabAddress => _defaultPrefabAddress;

            public IReadOnlyDictionary<string, IAudioInfo> CreateAudioInfoMapDataId()
            {
                Assert.IsNotNull(_audioInfos, "[AudioPlayerModuleConfig::CreateAudioInfoMapDataId] AudioInfos is null.");

                var audioInfoMap = new Dictionary<string, IAudioInfo>();

                if (_audioInfos is null)
                    return audioInfoMap;

                foreach (var audioInfo in _audioInfos)
                    audioInfoMap.Add(audioInfo.DataId, audioInfo);

                return audioInfoMap;
            }

            [Serializable]
            private class AudioInfo : IAudioInfo
            {
                [SerializeField]
                private string _dataId;

                [Space]
                [SerializeField]
                private string _clipAddress;

                [SerializeField]
                private string _prefabAddress;

                public string DataId => _dataId;

                public string ClipAddress => _clipAddress;
                public string PrefabAddress => _prefabAddress;
            }

            [Serializable]
            private class RestrictContinuousPlayImp : IRestrictContinuousPlay
            {
                [SerializeField]
                private bool _isEnable = true;

                [Space]
                [SerializeField]
                private float _duration = 0.1f;

                [SerializeField]
                private int _maxConsecutiveCount = 1;

                public bool IsEnable => _isEnable;

                public float Duration => _duration;
                public int MaxConsecutiveCount => _maxConsecutiveCount;
            }
        }
    }
}