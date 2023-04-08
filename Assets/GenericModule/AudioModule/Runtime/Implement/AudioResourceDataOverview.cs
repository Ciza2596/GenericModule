using System;
using UnityEngine;

namespace CizaAudioModule.Implement
{
    [CreateAssetMenu(fileName = "AudioResourceDataOverview", menuName = "Ciza/AudioModule/AudioResourceDataOverview")]
    public class AudioResourceDataOverview : ScriptableObject
    {
        [SerializeField]
        private AudioData[] _audioDatas;

        public IAudioData[] GetAudioDatas => _audioDatas;
        

        [Serializable]
        public class AudioData: IAudioData
        {
            [SerializeField]
            private string _key;

            [SerializeField] private GameObject _prefab;

            public string ClipDataId => _key;
            public string PrefabDataId { get; }
            public float SpatialBlend { get; }
            public GameObject Prefab => _prefab;
        }
    }
}
