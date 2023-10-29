using UnityEngine;
using UnityEngine.Assertions;

namespace CizaAudioModule
{
	public class Audio : MonoBehaviour, IAudio
	{
		private AudioSource _audioSource;

		private float _time;

		public string Id     { get; private set; }
		public string DataId { get; private set; }

		public string ClipAddress   { get; private set; }
		public string PrefabAddress { get; private set; }
		
		public bool  IsComplete => _time >= Duration;
		public bool  IsLoop     { get; private set; }
		public float Volume     => _audioSource.volume;
		public float Duration   { get; private set; }

		public GameObject GameObject => gameObject;

		public void Initialize(string prefabAddress)
		{
			PrefabAddress = prefabAddress;

			_audioSource      = GetComponentInChildren<AudioSource>();
			_audioSource.loop = false;
			Assert.IsNotNull(_audioSource, $"[Audio::Initialize] AudioSource is not found. Please check prefabDataId: {PrefabAddress}.");
		}

		public void Tick(float deltaTime) =>
			_time += deltaTime;

		public void Play(string id, string dataId, string clipAddress, AudioClip audioClip, float volume, bool isLoop)
		{
			SetParameter(id, dataId, clipAddress, audioClip, volume, isLoop);
			_time = 0;
			_audioSource.Play();

		}

		public void Continue()
		{
			_time = 0;
			_audioSource.Stop();
			_audioSource.Play();
		}

		public void Stop()
		{
			_audioSource.Stop();
			SetParameter();
		}

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
		private void SetParameter(string id = null, string dataId = null, string clipAddress = null, AudioClip audioClip = null, float volume = 0, bool isLoop = false)
		{
			Id     = id;
			DataId = dataId;

			ClipAddress       = clipAddress;
			_audioSource.clip = audioClip;

			Duration = audioClip is null ? 0 : audioClip.length;

			SetVolume(volume);

			SetIsLoop(isLoop);
		}
	}
}