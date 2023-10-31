using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace CizaAudioModule
{
	public class AudioPlayer
	{
		private readonly IAudioPlayerConfig _audioPlayerConfig;

		private readonly AudioModule _bgmModule;
		private readonly AudioModule _sfxModule;
		private readonly AudioModule _voiceModule;

		private Transform _root;

		public event Func<string, UniTask> OnChangedVoiceLocaleBefore;
		public event Func<string, UniTask> OnChangedVoiceLocale;

		// Id, DataId
		public event Action<string, string> OnBgmPlay;
		public event Action<string, string> OnBgmStop;
		public event Action<string, string> OnBgmComplete;

		public event Action<string, string> OnSfxPlay;
		public event Action<string, string> OnSfxStop;
		public event Action<string, string> OnSfxComplete;

		public event Action<string, string> OnVoicePlay;
		public event Action<string, string> OnVoiceStop;
		public event Action<string, string> OnVoiceComplete;

		public bool IsInitialized => _root != null && _bgmModule.IsInitialized && _sfxModule.IsInitialized && _voiceModule.IsInitialized;

		public bool TryGetMasterMixerGroup(out AudioMixerGroup masterMixerGroup)
		{
			if (_audioPlayerConfig.AudioMixer is null)
			{
				Debug.LogWarning("[AudioModule::TryGetMasterMixerGroup] AudioMixer is null.");
				masterMixerGroup = null;
				return false;
			}

			masterMixerGroup = _audioPlayerConfig.AudioMixer.FindMatchingGroups(_audioPlayerConfig.MasterMixerGroupPath).First();
			return masterMixerGroup != null;
		}

		public bool TryGetBgmMixerGroup(out AudioMixerGroup bgmMixerGroup) =>
			_bgmModule.TryGetAudioMixerGroup(out bgmMixerGroup);

		public bool TryGetSfxMixerGroup(out AudioMixerGroup sfxMixerGroup) =>
			_sfxModule.TryGetAudioMixerGroup(out sfxMixerGroup);

		public bool TryGetVoiceMixerGroup(out AudioMixerGroup voiceMixerGroup) =>
			_voiceModule.TryGetAudioMixerGroup(out voiceMixerGroup);

		public bool TryGetMasterVolume(out float volume)
		{
			if (_audioPlayerConfig.AudioMixer is null)
			{
				Debug.LogWarning("[AudioModule::TryGetVolume] AudioMixer is null.");
				volume = 0;
				return false;
			}

			return _audioPlayerConfig.AudioMixer.GetFloat(_audioPlayerConfig.MasterMixerParameter, out volume);
		}

		public bool TryGetBgmVolume(out float volume) =>
			_bgmModule.TryGetVolume(out volume);

		public bool TryGetSfxVolume(out float volume) =>
			_sfxModule.TryGetVolume(out volume);

		public bool TryGetVoiceVolume(out float volume) =>
			_voiceModule.TryGetVolume(out volume);

		public AudioPlayer(IAudioPlayerConfig audioPlayerConfig, IAssetProvider assetProvider, IVoiceAssetProvider voiceAssetProvider)
		{
			_audioPlayerConfig = audioPlayerConfig;

			_bgmModule   = new AudioModule(audioPlayerConfig.BgmModuleConfig, assetProvider, assetProvider, audioPlayerConfig.AudioMixer, _audioPlayerConfig.IsDontDestroyOnLoad);
			_sfxModule   = new AudioModule(audioPlayerConfig.SfxModuleConfig, assetProvider, assetProvider, audioPlayerConfig.AudioMixer, _audioPlayerConfig.IsDontDestroyOnLoad);
			_voiceModule = new AudioModule(audioPlayerConfig.VoiceModuleConfig, voiceAssetProvider, assetProvider, audioPlayerConfig.AudioMixer, _audioPlayerConfig.IsDontDestroyOnLoad);

			_bgmModule.OnPlay     += (bgmId, bgmDataId) => OnBgmPlay?.Invoke(bgmId, bgmDataId);
			_bgmModule.OnStop     += (bgmId, bgmDataId) => OnBgmStop?.Invoke(bgmId, bgmDataId);
			_bgmModule.OnComplete += (bgmId, bgmDataId) => OnBgmComplete?.Invoke(bgmId, bgmDataId);

			_sfxModule.OnPlay     += (sfxId, sfxDataId) => OnSfxPlay?.Invoke(sfxId, sfxDataId);
			_sfxModule.OnStop     += (sfxId, sfxDataId) => OnSfxStop?.Invoke(sfxId, sfxDataId);
			_sfxModule.OnComplete += (sfxId, sfxDataId) => OnSfxComplete?.Invoke(sfxId, sfxDataId);

			_voiceModule.OnPlay     += (voiceId, voiceDataId) => OnVoicePlay?.Invoke(voiceId, voiceDataId);
			_voiceModule.OnStop     += (voiceId, voiceDataId) => OnVoiceStop?.Invoke(voiceId, voiceDataId);
			_voiceModule.OnComplete += (voiceId, voiceDataId) => OnVoiceComplete?.Invoke(voiceId, voiceDataId);

			voiceAssetProvider.OnChangedLocaleBefore += m_OnChangedLocaleBefore;
			voiceAssetProvider.OnChangedLocale       += m_OnChangedLocale;

			UniTask m_OnChangedLocaleBefore(string locale) =>
				OnChangedVoiceLocaleBefore?.Invoke(locale) ?? UniTask.CompletedTask;

			UniTask m_OnChangedLocale(string locale) =>
				OnChangedVoiceLocale?.Invoke(locale) ?? UniTask.CompletedTask;
		}

		public void Initialize(Transform rootParent = null)
		{
			if (IsInitialized)
			{
				Debug.LogWarning("[AudioPlayer::Initialize] AudioPlayer is initialized.");
				return;
			}

			var rootGameObject = new GameObject(_audioPlayerConfig.RootName);
			if (_audioPlayerConfig.IsDontDestroyOnLoad)
				Object.DontDestroyOnLoad(rootGameObject);

			_root = rootGameObject.transform;

			if (rootParent != null)
				_root.SetParent(rootParent);

			_bgmModule.Initialize(_root);
			_sfxModule.Initialize(_root);
			_voiceModule.Initialize(_root);

			SetMasterVolume(_audioPlayerConfig.DefaultMasterVolume);
		}

		public void Release()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioPlayer::Release] AudioPlayer is not initialized.");
				return;
			}

			_bgmModule.Release();
			_sfxModule.Release();
			_voiceModule.Release();

			var rootGameObject = _root.gameObject;
			_root = null;
			DestroyOrImmediate(rootGameObject);
		}

		public void Tick(float deltaTime)
		{
			if (!IsInitialized)
				return;

			_bgmModule.Tick(deltaTime);
			_sfxModule.Tick(deltaTime);
			_voiceModule.Tick(deltaTime);
		}

		public void SetMasterVolume(float volume)
		{
			if (_audioPlayerConfig.AudioMixer is null)
			{
				Debug.LogWarning("[AudioPlayer::SetMasterVolume] AudioMixer is null.");
				return;
			}

			_audioPlayerConfig.AudioMixer.SetFloat(_audioPlayerConfig.MasterMixerParameter, m_GetLinearToLogarithmicScale(volume));

			float m_GetLinearToLogarithmicScale(float value) =>
				Mathf.Log(Mathf.Clamp(value, 0.001f, 1)) * 20.0f;
		}

		public async UniTask StopAllAsync(float fadeTime = 0)
		{
			await StopAllBgmAsync(fadeTime);
			await StopAllSfxAsync(fadeTime);
			await StopAllVoiceAsync(fadeTime);
		}

		#region Bgm

		public void SetBgmVolume(float volume) =>
			_bgmModule.SetVolume(volume);

		public UniTask LoadBgmAssetAsync(string bgmDataId, CancellationToken cancellationToken = default) =>
			_bgmModule.LoadAudioAssetAsync(bgmDataId, cancellationToken);

		public void UnloadBgmAsset(string bgmDataId) =>
			_bgmModule.UnloadAudioAsset(bgmDataId);

		public UniTask<string> PlayBgmAsync(string bgmDataId, float volume = 1, float fadeTime = 0, bool isLoop = false, Vector3 position = default) =>
			_bgmModule.PlayAsync(bgmDataId, volume, fadeTime, isLoop, position);

		public UniTask ModifyBgmAsync(string bgmId, float volume, bool isLoop, float time = 0) =>
			_bgmModule.ModifyAsync(bgmId, volume, isLoop, time);

		public UniTask ModifyBgmAsync(string bgmId, float volume, float time = 0) =>
			_bgmModule.ModifyAsync(bgmId, volume, time);

		public void PauseBgm(string bgmId) =>
			_bgmModule.Pause(bgmId);

		public void ResumeBgm(string bgmId) =>
			_bgmModule.Resume(bgmId);

		public UniTask StopBgmAsync(string bgmId, float fadeTime = 0) =>
			_bgmModule.StopAsync(bgmId, fadeTime);

		public UniTask StopAllBgmAsync(float fadeTime = 0) =>
			_bgmModule.StopAllAsync(fadeTime);

		#endregion

		#region Sfx

		public void SetSfxVolume(float volume) =>
			_sfxModule.SetVolume(volume);

		public UniTask LoadSfxAssetAsync(string sfxDataId, CancellationToken cancellationToken = default) =>
			_sfxModule.LoadAudioAssetAsync(sfxDataId, cancellationToken);

		public void UnloadSfxAsset(string sfxDataId) =>
			_sfxModule.UnloadAudioAsset(sfxDataId);

		public UniTask<string> PlaySfxAsync(string sfxDataId, float volume = 1, float fadeTime = 0, bool isLoop = false, Vector3 position = default) =>
			_sfxModule.PlayAsync(sfxDataId, volume, fadeTime, isLoop, position);

		public UniTask ModifySfxAsync(string sfxId, float volume, bool isLoop, float time = 0) =>
			_sfxModule.ModifyAsync(sfxId, volume, isLoop, time);

		public UniTask ModifySfxAsync(string sfxId, float volume, float time = 0) =>
			_sfxModule.ModifyAsync(sfxId, volume, time);

		public void PauseSfx(string sfxId) =>
			_sfxModule.Pause(sfxId);

		public void ResumeSfx(string sfxId) =>
			_sfxModule.Resume(sfxId);

		public UniTask StopSfxAsync(string sfxId, float fadeTime = 0) =>
			_sfxModule.StopAsync(sfxId, fadeTime);

		public UniTask StopAllSfxAsync(float fadeTime = 0) =>
			_sfxModule.StopAllAsync(fadeTime);

		#endregion

		#region Voice

		public void SetVoiceVolume(float volume) =>
			_voiceModule.SetVolume(volume);

		public UniTask LoadVoiceAssetAsync(string voiceDataId, CancellationToken cancellationToken = default) =>
			_voiceModule.LoadAudioAssetAsync(voiceDataId, cancellationToken);

		public void UnloadVoiceAsset(string voiceDataId) =>
			_voiceModule.UnloadAudioAsset(voiceDataId);

		public UniTask<string> PlayVoiceAsync(string voiceDataId, float volume = 1, float fadeTime = 0, bool isLoop = false, Vector3 position = default) =>
			_voiceModule.PlayAsync(voiceDataId, volume, fadeTime, isLoop, position);

		public UniTask ModifyVoiceAsync(string voiceId, float volume, bool isLoop, float time = 0) =>
			_voiceModule.ModifyAsync(voiceId, volume, isLoop, time);

		public UniTask ModifyVoiceAsync(string voiceId, float volume, float time = 0) =>
			_voiceModule.ModifyAsync(voiceId, volume, time);

		public void PauseVoice(string voiceId) =>
			_voiceModule.Pause(voiceId);

		public void ResumeVoice(string voiceId) =>
			_voiceModule.Resume(voiceId);

		public UniTask StopVoiceAsync(string voiceId, float fadeTime = 0) =>
			_voiceModule.StopAsync(voiceId, fadeTime);

		public UniTask StopAllVoiceAsync(float fadeTime = 0) =>
			_voiceModule.StopAllAsync(fadeTime);

		#endregion

		private void DestroyOrImmediate(Object obj)
		{
			if (Application.isPlaying)
				Object.Destroy(obj);
			else
				Object.DestroyImmediate(obj);
		}
	}
}
