using UnityEngine;
using UnityEngine.Audio;

namespace CizaAudioModule
{
	public interface IAudio : IAudioReadModel
	{
		GameObject GameObject { get; }
		void Initialize(string prefabAddress, AudioMixerGroup audioMixerGroup);

		void Tick(float deltaTime);

		void Play(string userId, string id, string dataId, bool isOverridable, bool isAutoDespawn, string callerId, bool isRecord, string clipAddress, AudioClip audioClip, float volume, bool isLoop);


		void Continue();
		void Stop();

		void Resume();
		void Pause();
		
		void SetVolume(float volume);
		void SetCurrentVolume(float currentVolume);
		void SetIsLoop(bool isLoop);

		void SetTime(float time);
	}
}