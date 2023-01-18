using UnityEngine;
using UnityEngine.Assertions;

namespace AudioModule
{
    public class AudioData
    {
        //private variable
        private Transform _poolTransform;

        private AudioSource _audioSource;
        private Transform _selfTransform;


        //public variable
        public string Id { get; private set; }
        public string Key { get; private set; }
        public float Volume => _audioSource.volume;
        public float OriginVolume { get; }
        public float Duration { get; }


        //public method
        public AudioData(string id, string key, AudioSource audioSource, Transform poolTransform)
        {
            _audioSource = audioSource;

            _selfTransform = _audioSource.transform;
            _poolTransform = poolTransform;

            OriginVolume = _audioSource.volume;
            Id = id;
            Key = key;


            var clip = _audioSource.clip;
            
            Duration = clip is null ? 0 : clip.length;
        }

        public void SetVolume(float volume) => _audioSource.volume = volume;

        public void Play(Vector3 localPosition, Transform parentTransform)
        {
            _selfTransform.SetParent(parentTransform);

            if (localPosition != default)
                _selfTransform.localPosition = localPosition;


            if(_audioSource.clip is null)
                Debug.LogWarning($"[AudioData::Play] Clip is null. Please check Key: {Key} audioPrefab.");

            _audioSource.gameObject.SetActive(true);
            _audioSource.Play();
        }

        public void Resume() => _audioSource.Play();

        public void Pause() => _audioSource.Pause();

        public void Stop()
        {
            _audioSource.Stop();
            _selfTransform.SetParent(_poolTransform);
            _audioSource.gameObject.SetActive(false);
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
            Object.DestroyImmediate(gameObject);
        }
    }
}