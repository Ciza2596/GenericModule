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
        public float Volume => _audioSource.volume;
        public float OriginVolume { get; }
        public float Duration { get;}


        //public method
        public AudioData(string id, string key, AudioSource audioSource, Transform poolTransform)
        {
            _audioSource = audioSource;

            _selfTransform = _audioSource.transform;
            _poolTransform = poolTransform;
            
            OriginVolume = _audioSource.volume;
            Id = id;
            Key = key;
            
            Duration = _audioSource.clip.length;
        }

        public void SetVolume(float volume) => _audioSource.volume = volume;
        
        public void Play(Vector3 localPosition, Transform parentTransform)
        {
            _selfTransform.SetParent(parentTransform);
            
            if(localPosition != default)
                _selfTransform.localPosition = localPosition;


            Assert.IsNotNull(_audioSource.clip,
                $"[AudioData::Play] Clip is null. Please check Key: {Key} audioPrefab.");
            
            _audioSource.Play();
        }
        
        public void Resume() =>_audioSource.Play();

        public void Pause() => _audioSource.Pause();

        public void Stop()
        {
            _audioSource.Stop();
            _selfTransform.SetParent(_poolTransform);
        }

        public void Release()
        {
            Stop();
            
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