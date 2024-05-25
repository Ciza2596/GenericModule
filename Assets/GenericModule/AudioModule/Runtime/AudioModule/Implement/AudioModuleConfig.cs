using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaAudioModule.Implement
{
    [CreateAssetMenu(fileName = "AudioModuleConfig", menuName = "Ciza/AudioModule/AudioModuleConfig")]
    public class AudioModuleConfig : ScriptableObject, IAudioModuleConfig
    {
        [SerializeField]
        private string _poolRootName = "[AudioModule]";

        [Space]
        [SerializeField]
        private string _poolPrefix = "";

        [SerializeField]
        private string _poolSuffix = "s";

        [Space]
        [SerializeField]
        private string _audioMixerGroupPath = "Master";

        [SerializeField]
        private string _audioMixerParameter = "Master";

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

        public string PoolRootName => _poolRootName;

        public string PoolPrefix => _poolPrefix;
        public string PoolSuffix => _poolSuffix;

        public string AudioMixerGroupPath => _audioMixerGroupPath;
        public string AudioMixerParameter => _audioMixerParameter;
        public float DefaultVolume => _defaultVolume;

        public IRestrictContinuousPlay RestrictContinuousPlay => _restrictContinuousPlay;

        public string DefaultPrefabAddress => _defaultPrefabAddress;

        public IReadOnlyDictionary<string, IAudioInfo> CreateAudioInfoMapDataId()
        {
            Assert.IsNotNull(_audioInfos, "[AudioModuleConfig::CreateAudioInfoMapDataId] AudioInfos is null.");

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