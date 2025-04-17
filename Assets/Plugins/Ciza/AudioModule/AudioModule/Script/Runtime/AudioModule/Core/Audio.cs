using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

namespace CizaAudioModule
{
	public class Audio : MonoBehaviour, IAudio
	{
		public const float DURATION_ERROR = 0.00001f;

		private AudioSource _audioSource;

		private string _callerId;

		private float _time;

		public string Id { get; private set; }
		public string DataId { get; private set; }

		public string CallerId => !string.IsNullOrEmpty(_callerId) ? _callerId : string.Empty;
		public bool IsAutoDespawn { get; private set; }

		public string UserId { get; private set; }

		public string ClipAddress { get; private set; }
		public string PrefabAddress { get; private set; }

		public bool IsComplete => IsPlaying && _time >= Duration;

		public bool IsLoop { get; private set; }
		public float Volume => _audioSource.volume;

		public float Duration { get; private set; }
		public float Time => _time;
		public bool IsPlaying { get; private set; }

		public GameObject GameObject => gameObject;

		public void Initialize(string prefabAddress, AudioMixerGroup audioMixerGroup)
		{
			PrefabAddress = prefabAddress;

			_audioSource = GetComponentInChildren<AudioSource>();
			_audioSource.outputAudioMixerGroup = audioMixerGroup;
			_audioSource.loop = false;
			Assert.IsNotNull(_audioSource, $"[Audio::Initialize] AudioSource is not found. Please check prefabDataId: {PrefabAddress}.");
		}

		public void Tick(float deltaTime)
		{
			if (IsPlaying)
				SetTime(Time + deltaTime);
		}

		public void Play(string userId, string id, string dataId, bool isAutoDespawn, string callerId, string clipAddress, AudioClip audioClip, float volume, bool isLoop)
		{
			SetParameter(userId, id, dataId, isAutoDespawn, callerId, clipAddress, audioClip, volume, isLoop);
			SetTime(0);
		}

		public void Continue()
		{
			SetTime(0);
			_audioSource.Stop();
			_audioSource.Play();
		}

		public void Stop()
		{
			IsPlaying = false;
			_audioSource.Stop();
			SetParameter(string.Empty, string.Empty, string.Empty, true, string.Empty, string.Empty, null, 1, false);
			SetTime(0);
		}

		public void Resume()
		{
			if (!IsPlaying)
			{
				IsPlaying = true;
				_audioSource.Play();
			}
		}

		public void Pause()
		{
			if (IsPlaying)
			{
				IsPlaying = false;
				_audioSource.Pause();
			}
		}


		public void SetVolume(float volume) =>
			_audioSource.volume = volume;

		public void SetIsLoop(bool isLoop) =>
			IsLoop = isLoop;

		public void SetTime(float time)
		{
			_time = time;
			var minTime = Mathf.Min(time, Duration);
			if (_audioSource.clip != null && (time <= 0 || Mathf.Abs(_audioSource.time - minTime) > 0.02f))
				_audioSource.time = minTime;
		}


		// private method
		private void SetParameter(string userId, string id, string dataId, bool isAutoDespawn, string callerId, string clipAddress, AudioClip audioClip, float volume, bool isLoop)
		{
			UserId = userId;

			Id = id;
			DataId = dataId;

			IsAutoDespawn = isAutoDespawn;
			_callerId = callerId;

			ClipAddress = clipAddress;
			_audioSource.clip = audioClip;

			Duration = audioClip != null && audioClip.length > DURATION_ERROR ? audioClip.length - DURATION_ERROR : 0;

			SetVolume(volume);

			SetIsLoop(isLoop);
		}
	}
}