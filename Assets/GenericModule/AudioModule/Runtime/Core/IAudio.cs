using UnityEngine;

namespace CizaAudioModule
{
    public interface IAudio : IAudioReadModel
    {
        GameObject GameObject { get; }
        void Initialize(string prefabDataId);
        void Play(string id, string clipDataId, AudioClip audioClip, float volume, bool isLoop);
        void Stop();
        void Resume();
        void Pause();
        void SetVolume(float volume);
        
    }
}