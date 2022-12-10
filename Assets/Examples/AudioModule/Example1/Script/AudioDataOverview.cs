using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AudioModule.Example1
{
    [CreateAssetMenu(fileName = "AudioDataOverview", menuName = "AudioModule/AudioDataOverview")]
    public class AudioDataOverview : ScriptableObject
    {
        [SerializeField]
        private List<AudioData> _audioDatas;

        public List<IAudioData> GetAudioDatas => _audioDatas.ToList<IAudioData>();
        

        [Serializable]
        public class AudioData: IAudioData
        {
            [SerializeField]
            private string _key;

            [SerializeField] private GameObject _prefab;

            public string Key => _key;
            public GameObject Prefab => _prefab;
        }
    }
}
