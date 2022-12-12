using UnityEngine;
using UnityEngine.Assertions;

namespace AudioModule
{
    public struct AudioData
    {
        //private variable
        private AudioSource _audioSource;

        private Transform _selfTransform;
        private Transform _poolTransform;
        
        
        //public variable
        public string Id { get; private set; }
        public string Key { get; private set; }
        public float OriginVolume { get; }
        

        //public method
        public AudioData(string id, string key, AudioSource audioSource, Transform poolTransform)
        {
            _audioSource = audioSource;

            _selfTransform = _audioSource.transform;
            _poolTransform = poolTransform;
            
            OriginVolume = _audioSource.volume;
            Id = id;
            Key = key;
        }

        public void Play(Vector3 position, Transform parentTransform)
        {
            if(position != default)
                _selfTransform.position = position;
            
            if (parentTransform != null)
                _selfTransform.SetParent(parentTransform);
            
            
            Assert.IsNotNull(_audioSource.clip,
                $"[AudioData::Play] Clip is null. Please check Key: {Key} audioPrefab.");
            
            _audioSource.Play();
        }

        public void SetVolume(float volume) => _audioSource.volume = volume;

        public void Stop()
        {
            _audioSource.Stop();
            _selfTransform.SetParent(_poolTransform);
        }

        public void Release()
        {
            Id = null;
            Key = null;

            _selfTransform = null;
            _poolTransform = null;

            var audioSource = _audioSource;
            _audioSource = null;

            var gameObject = audioSource.gameObject;
            Object.Destroy(gameObject);
        }
    }
}