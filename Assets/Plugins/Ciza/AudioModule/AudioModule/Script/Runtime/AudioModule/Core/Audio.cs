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

		public string UserId { get; private set; }

		public string Id { get; private set; }
		public string DataId { get; private set; }

		public bool IsOverridable { get; private set; }
		public bool IsAutoDespawn { get; private set; }

		public string CallerId => !string.IsNullOrEmpty(_callerId) ? _callerId : string.Empty;
		public bool IsRecord { get; private set; }


		public string ClipAddress { get; private set; }
		public string PrefabAddress { get; private set; }

		public bool IsComplete => IsPlaying && _time >= Duration && (IsSyncTime || !_audioSource.isPlaying);

		public bool IsLoop { get; private set; }

		public float Volume { get; private set; }
		public float CurrentVolume => _audioSource.volume;

		public bool IsSyncTime { get; private set; }

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

		public void Play(string userId, string id, string dataId, bool isOverridable, bool isAutoDespawn, string callerId, bool isRecord, string clipAddress, AudioClip audioClip, float volume, bool isLoop, bool isSyncTime)
		{
			SetParameter(userId, id, dataId, isOverridable, isAutoDespawn, callerId, isRecord, clipAddress, audioClip, volume, isLoop, isSyncTime);
			SetTime(0);
		}

		public void Continue()
		{
			if (!IsPlaying)
				IsPlaying = true;

			SetTime(0);
			_audioSource.Stop();
			_audioSource.Play();
		}

		public void Stop()
		{
			IsPlaying = false;
			_audioSource.Stop();
			SetParameter(string.Empty, string.Empty, string.Empty, false, true, string.Empty, false, string.Empty, null, 1, false, false);
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

		public virtual void SetVolume(float volume) =>
			Volume = volume;

		public void SetCurrentVolume(float currentVolume) =>
			_audioSource.volume = currentVolume;

		public void SetIsLoop(bool isLoop) =>
			IsLoop = isLoop;

		public void SetTime(float time, bool isSyncTime)
		{
			_time = time;
			var targetTime = Mathf.Min(time, Duration);
			if (isSyncTime && _audioSource.clip != null && (time <= 0 || Mathf.Abs(_audioSource.time - targetTime) > 0.01f))
				_audioSource.time = targetTime;
		}

		// private method
		private void SetTime(float time) =>
			SetTime(time, IsSyncTime);

		private void SetParameter(string userId, string id, string dataId, bool isOverridable, bool isAutoDespawn, string callerId, bool isRecord, string clipAddress, AudioClip audioClip, float volume, bool isLoop, bool isSyncTime)
		{
			UserId = userId;

			Id = id;
			DataId = dataId;

			IsOverridable = isOverridable;
			IsAutoDespawn = isAutoDespawn;

			_callerId = callerId;
			IsRecord = isRecord;

			ClipAddress = clipAddress;
			_audioSource.clip = audioClip;

			Duration = audioClip != null && audioClip.length > DURATION_ERROR ? audioClip.length - DURATION_ERROR : 0;

			SetVolume(volume);
			SetCurrentVolume(volume);

			SetIsLoop(isLoop);

			IsSyncTime = isSyncTime;
		}
	}
}