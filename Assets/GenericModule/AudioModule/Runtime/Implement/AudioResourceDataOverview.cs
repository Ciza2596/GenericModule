using System;
using UnityEngine;

namespace CizaAudioModule.Implement
{
    [CreateAssetMenu(fileName = "AudioResourceDataOverview", menuName = "Ciza/AudioModule/AudioResourceDataOverview")]
    public class AudioResourceDataOverview : ScriptableObject
    {
        [SerializeField]
        private AudioResourceData[] _audioDatas;

        public IAudioResourceData[] GetAudioDatas => _audioDatas;
        

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
