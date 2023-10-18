using UnityEngine;

namespace CizaAudioModule
{
	public interface IAudio : IAudioReadModel
	{
		GameObject GameObject { get; }
		void Initialize(string prefabAddress);
		void Play(string       id, string dataId, string clipAddress, AudioClip audioClip, float volume, bool isLoop);
		void Stop();
		void Resume();
		void Pause();
		void SetVolume(float volume);
	}
}
