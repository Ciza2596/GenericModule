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
        private List<AudioResourceData> _audioDatas;

        public List<IAudioResourceData> GetAudioDatas => _audioDatas.ToList<IAudioResourceData>();
        

        [Serializable]
        public class AudioResourceData: IAudioResourceData
        {
            [SerializeField]
            private string _key;

            [SerializeField] private GameObject _prefab;

            public string Key => _key;
            public GameObject Prefab => _prefab;
        }
    }
}
