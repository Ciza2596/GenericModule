using UnityEngine;

namespace CizaAudioModule
{
    public abstract class BaseAudio: MonoBehaviour, IAudio
    {
        [SerializeField] private AudioSource _audioSource;
        
        
        public string ClipDataId { get; private set; }
        public string PrefabDataId { get; private set; }
        public float SpatialBlend { get; private set; }
        
        
        public string Id { get; private set; }

        public GameObject GameObject => gameObject;


        public void Initialize(string prefabDataId) =>
            PrefabDataId = prefabDataId;
        
        public void Play(string id, string clipDataId, AudioClip audioClip, float spatialBlend, Vector3 position, Transform parentTransform, float volume)
        {
            
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }

        public void Resume()
        {
            throw new System.NotImplementedException();
        }

        public void Pause()
        {
            throw new System.NotImplementedException();
        }

        public void SetVolume(float volume)
        {
            throw new System.NotImplementedException();
        }
    }
}