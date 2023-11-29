using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

namespace CizaAudioModule
{
	public class Audio : MonoBehaviour, IAudio
	{
		private AudioSource _audioSource;

		private string _callerId;

		private float _time;

		public string Id     { get; private set; }
		public string DataId { get; private set; }

		public string CallerId => !string.IsNullOrEmpty(_callerId) ? _callerId : string.Empty;

		public string ClipAddress   { get; private set; }
		public string PrefabAddress { get; private set; }

		public bool IsComplete => _time >= Duration;

		public bool  IsLoop   { get; private set; }
		public float Volume   => _audioSource.volume;
		public float Duration { get; private set; }

		public GameObject GameObject => gameObject;

		public void Initialize(string prefabAddress, AudioMixerGroup audioMixerGroup)
		{
			PrefabAddress = prefabAddress;

			_audioSource                       = GetComponentInChildren<AudioSource>();
			_audioSource.outputAudioMixerGroup = audioMixerGroup;
			_audioSource.loop = false;
			Assert.IsNotNull(_audioSource, $"[Audio::Initialize] AudioSource is not found. Please check prefabDataId: {PrefabAddress}.");
		}

		public void Tick(float deltaTime) =>
			_time += deltaTime;

		public void Play(string id, string dataId, string callerId, string clipAddress, AudioClip audioClip, float volume, bool isLoop)
		{
			SetParameter(id, dataId, callerId, clipAddress, audioClip, volume, isLoop);
			_time = 0;
			_audioSource.Play();
		}

		public void Continue()
		{
			_time = 0;
			_audioSource.Stop();
			_audioSource.Play();
		}

		public void Stop() =>
			_audioSource.Stop();

		public void Resume()
		{
			if (!_audioSource.isPlaying)
				_audioSource.Play();
		}

		public void Pause() =>
			_audioSource.Pause();

		public void SetVolume(float volume) =>
			_audioSource.volume = volume;

		public void SetIsLoop(bool isLoop) =>
			IsLoop = isLoop;

		// private method
		private void SetParameter(string id, string dataId, string callerId, string clipAddress, AudioClip audioClip, float volume, bool isLoop)
		{
			Id     = id;
			DataId = dataId;

			_callerId = callerId;

			ClipAddress       = clipAddress;
			_audioSource.clip = audioClip;

			Duration = audioClip is null ? 0 : audioClip.length;

			SetVolume(volume);

			SetIsLoop(isLoop);
		}
	}
}
