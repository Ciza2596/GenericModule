using System;
using UnityEngine;

namespace CizaAudioModule
{
    public abstract class BaseAudio : MonoBehaviour, IAudio
    {
        [SerializeField] private AudioSource _audioSource;

        public string Id { get; private set; }

        public string ClipDataId { get; private set; }
        public string PrefabDataId { get; private set; }
        public float SpatialBlend { get; private set; }

        public float Volume => _audioSource.volume;
        
        public GameObject GameObject => gameObject;
        

        public void Initialize(string prefabDataId) =>
            PrefabDataId = prefabDataId;

        public void Play(string id, string clipDataId, AudioClip audioClip, float spatialBlend, float volume)
        {
            SetParameter(id, clipDataId, audioClip, spatialBlend, volume);
            _audioSource.Play();
        }

        public void Stop()
        {
            _audioSource.Stop();
            SetParameter();
        }

        public void Resume()
        {
            if(!_audioSource.isPlaying) 
                _audioSource.Play();
        }

        public void Pause() =>
            _audioSource.Pause();

        public void SetVolume(float volume) => 
            _audioSource.volume = volume;


        // private method
        private void SetParameter(string id = null, string clipDataId = null, AudioClip audioClip = null, float spatialBlend = 0, float volume = 0)
        {
            Id = id;
            ClipDataId = clipDataId;
            _audioSource.clip = audioClip;

            SpatialBlend = spatialBlend;
            _audioSource.spatialBlend = SpatialBlend;

            SetVolume(volume);
        }
    }
}