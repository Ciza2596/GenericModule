using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaAudioModule.Implement
{
    [CreateAssetMenu(fileName = "AudioModuleConfig", menuName = "Ciza/AudioModule/AudioModuleConfig")]
    public class AudioModuleConfig : ScriptableObject, IAudioModuleConfig
    {
        [SerializeField] private string _audioMixerVolumeParameter = "Master";
        [Space] [SerializeField] private string _poolRootName = "[AudioModulePoolRoot]";
        [SerializeField] private string _poolPrefix = "[";
        [SerializeField] private string _poolSuffix = "s]";
        
        [SerializeField]
        private AudioData[] _audioDatas;


        public string                                  AudioMixerVolumeParameter => _audioMixerVolumeParameter;
        public string                                  PoolRootName              => _poolRootName;
        public string                                  PoolPrefix                => _poolPrefix;
        public string                                  PoolSuffix                => _poolSuffix;
        public IReadOnlyDictionary<string, IAudioData> CreateAudioDataMap()
        {
            Assert.IsNotNull(_audioDatas, "[AudioModuleConfig::GetAudioDataMap] AudioDatas is null.");
            
            var audioDataMap = new Dictionary<string, IAudioData>();

            foreach (var audioData in _audioDatas)
                audioDataMap.Add(audioData.ClipDataId, audioData);

            return audioDataMap;
        }
        
        
        [Serializable]
        private class AudioData : IAudioData
        {
            [SerializeField]               private string _clipDataId;
            [SerializeField]               private string _prefabDataId;
            [Range(0, 1)] [SerializeField] private float  _spatialBlend;


            public string ClipDataId   => _clipDataId;
            public string PrefabDataId => _prefabDataId;
            public float  SpatialBlend => _spatialBlend;
        }
    }
}