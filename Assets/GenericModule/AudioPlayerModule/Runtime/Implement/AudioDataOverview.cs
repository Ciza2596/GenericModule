using System;
using System.Collections.Generic;
using CizaAudioModule;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaAudioPlayerModule.Implement
{
    [CreateAssetMenu(fileName = "AudioDataOverview", menuName = "Ciza/AudioPlayerModule/AudioDataOverview", order = -100)]
    public class AudioDataOverview : ScriptableObject
    {
        [SerializeField]
        private AudioData[] _audioDatas;
        
        public Dictionary<string, IAudioData> GetAudioDataMap()
        {
            Assert.IsNotNull(_audioDatas, "[AudioDataOverview::GetAudioDataMap] AudioDatas is null.");
            
            var audioDataMap = new Dictionary<string, IAudioData>();

            foreach (var audioData in _audioDatas)
                audioDataMap.Add(audioData.ClipDataId, audioData);

            return audioDataMap;
        }

        [Serializable]
        private class AudioData : IAudioData
        {
            [SerializeField] private string _clipDataId;
            [SerializeField] private string _prefabDataId;
            [Range(0, 1)] [SerializeField] private float _spatialBlend;


            public string ClipDataId => _clipDataId;
            public string PrefabDataId => _prefabDataId;
            public float SpatialBlend => _spatialBlend;
        }
    }
}