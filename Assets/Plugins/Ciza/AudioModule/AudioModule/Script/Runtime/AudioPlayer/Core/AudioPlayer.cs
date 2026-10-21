using System;
using System.Collections.Generic;
using System.Linq;
using CizaAsync;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace CizaAudioModule
{
	public class AudioPlayer : MenuAudioController.IAudioPlayer
	{
		private readonly IAudioPlayerConfig _audioPlayerConfig;

		private readonly AudioModule _bgmModule;
		private readonly AudioModule _sfxModule;
		private readonly AudioModule _voiceModule;

		private Transform _root;
		private MenuAudioController.IAudioPlayer _audioPlayerImplementation;

		public event Action OnInitialize;
		public event Action OnRelease;


		public event Func<string, AsyncToken, Awaitable> OnChangedVoiceLocaleBeforeAsync;
		public event Func<string, AsyncToken, Awaitable> OnChangedVoiceLocaleAsync;

		// CallerId, Id, DataId, UserId, IsOverridable, IsRecord
		public event Action<string, string, string, string, bool, bool> OnBgmSpawn;
		public event Action<string, string, string> OnBgmStop;
		public event Action<string, string, string> OnBgmComplete;

		public event Action<string, string, string, string, bool, bool> OnSfxSpawn;
		public event Action<string, string, string> OnSfxStop;
		public event Action<string, string, string> OnSfxComplete;

		// CallerId, Id, DataId, UserId, IsOverridable, IsRecord, ChannelDataId
		public event Action<string, string, string, string, bool, bool, string> OnVoiceSpawn;
		public event Action<string, string, string> OnVoiceStop;
		public event Action<string, string, string> OnVoiceComplete;

		public virtual bool IsInitialized => _root != null && _bgmModule.IsInitialized && _sfxModule.IsInitialized && _voiceModule.IsInitialized;

		public virtual string[] AllBgmInfoDataIds => _bgmModule.AudioInfoDataIds;
		public virtual string[] AllSfxInfoDataIds => _sfxModule.AudioInfoDataIds;

		public virtual string[] AllVoiceInfoDataIds => _voiceModule.AudioInfoDataIds;
		public virtual string[] AllVoiceChannelDataIds => _voiceModule.ChannelDataIds;

		#region Group

		public virtual bool TryGetMasterMixerGroup(out AudioMixerGroup masterMixerGroup)
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

		#region Bgm

		public virtual bool TryGetBgmMixerGroup(out AudioMixerGroup bgmMixerGroup) =>
			_bgmModule.TryGetMixerGroup(out bgmMixerGroup);

		public virtual bool TryGetBgmMixerGroup(string bgmGroupPath, out AudioMixerGroup bgmMixerGroup) =>
			_bgmModule.TryGetMixerGroup(bgmGroupPath, out bgmMixerGroup);

		#endregion


		#region Sfx

		public virtual bool TryGetSfxMixerGroup(out AudioMixerGroup sfxMixerGroup) =>
			_sfxModule.TryGetMixerGroup(out sfxMixerGroup);

		public virtual bool TryGetSfxMixerGroup(string sfxGroupPath, out AudioMixerGroup sfxMixerGroup) =>
			_bgmModule.TryGetMixerGroup(sfxGroupPath, out sfxMixerGroup);

		#endregion


		#region Voice

		public virtual bool TryGetVoiceMixerGroup(out AudioMixerGroup voiceMixerGroup) =>
			_voiceModule.TryGetMixerGroup(out voiceMixerGroup);

		public virtual bool TryGetVoiceMixerGroup(string voiceGroupPath, out AudioMixerGroup voiceMixerGroup) =>
			_voiceModule.TryGetMixerGroup(voiceGroupPath, out voiceMixerGroup);

		public virtual bool TryGetVoiceExtraChannelMixerGroup(out AudioMixerGroup voiceMixerGroup) =>
			_voiceModule.TryGetExtraChannelMixerGroup(out voiceMixerGroup);

		public virtual bool TryGetVoiceChannelMixerGroup(string channelDataId, out AudioMixerGroup voiceMixerGroup) =>
			_voiceModule.TryGetChannelMixerGroup(channelDataId, out voiceMixerGroup);

		#endregion

		#endregion

		public virtual float MinMasterMixerVolume => _audioPlayerConfig.MinMasterMixerVolume;
		public virtual float MaxMasterMixerVolume => _audioPlayerConfig.MaxMasterMixerVolume;

		public virtual float DefaultMasterMixerVolume =>
			_audioPlayerConfig.DefaultMasterMixerVolume;

		public virtual bool TryGetMasterMixerVolume(out float volume)
		{
			if (_audioPlayerConfig.AudioMixer is null)
			{
				Debug.LogWarning("[AudioModule::TryGetVolume] AudioMixer is null.");
				volume = 0;
				return false;
			}

			return _audioPlayerConfig.AudioMixer.GetFloat(_audioPlayerConfig.MasterMixerParameter, out volume);
		}

		#region Bgm

		public virtual float MinBgmMixerVolume =>
			_bgmModule.MinMixerVolume;

		public virtual float MaxBgmMixerVolume =>
			_bgmModule.MaxMixerVolume;

		public virtual float DefaultBgmMixerVolume =>
			_bgmModule.DefaultMixerVolume;

		public virtual bool TryGetBgmMixerVolume(out float volume) =>
			_bgmModule.TryGetMixerVolume(out volume);

		public virtual bool TryGetBgmMixerVolume(string parameter, out float volume) =>
			_bgmModule.TryGetMixerVolume(parameter, out volume);

		public virtual bool CheckHasBgmDataIdInConfig(string voiceDataId) =>
			_bgmModule.CheckIsAudioDataIdInConfig(voiceDataId);

		public virtual bool CheckHasBgm(string bgmId) =>
			_bgmModule.TryGetAudioReadModel(bgmId, out _);

		public virtual bool CheckIsBgmPlaying(string bgmId) =>
			_bgmModule.CheckIsPlaying(bgmId);

		public virtual bool TryGetBgmReadModel(string bgmId, out IAudioReadModel bgmReadModel) =>
			_bgmModule.TryGetAudioReadModel(bgmId, out bgmReadModel);

		public virtual bool TryGetBgmVolume(string bgmId, out float volume) =>
			_bgmModule.TryGetAudioVolume(bgmId, out volume);

		public virtual bool TryGetBgmIsOverridable(string bgmId, out bool isOverridable) =>
			_bgmModule.TryGetAudioIsOverridable(bgmId, out isOverridable);

		#endregion


		#region Sfx

		public virtual float MinSfxMixerVolume =>
			_sfxModule.MinMixerVolume;

		public virtual float MaxSfxMixerVolume =>
			_sfxModule.MaxMixerVolume;

		public virtual float DefaultSfxMixerVolume =>
			_sfxModule.DefaultMixerVolume;

		public virtual bool TryGetSfxMixerVolume(out float volume) =>
			_sfxModule.TryGetMixerVolume(out volume);

		public virtual bool TryGetSfxMixerVolume(string parameter, out float volume) =>
			_sfxModule.TryGetMixerVolume(parameter, out volume);

		public virtual bool CheckHasSfxDataIdInConfig(string sfxDataId) =>
			_sfxModule.CheckIsAudioDataIdInConfig(sfxDataId);

		public virtual bool CheckHasSfx(string sfxId) =>
			_sfxModule.TryGetAudioReadModel(sfxId, out _);

		public virtual bool CheckIsSfxPlaying(string sfxId) =>
			_sfxModule.CheckIsPlaying(sfxId);

		public virtual bool CheckIsSfxPause(string sfxId) =>
			_sfxModule.CheckIsPause(sfxId);

		public virtual float GetSfxDuration(string sfxDataId) =>
			_sfxModule.GetDuration(sfxDataId);

		public virtual bool TryGetSfxReadModel(string sfxId, out IAudioReadModel sfxReadModel) =>
			_sfxModule.TryGetAudioReadModel(sfxId, out sfxReadModel);

		public virtual bool TryGetSfxVolume(string sfxId, out float volume) =>
			_sfxModule.TryGetAudioVolume(sfxId, out volume);

		public virtual bool TryGetSfxIsOverridable(string sfxId, out bool isOverridable) =>
			_sfxModule.TryGetAudioIsOverridable(sfxId, out isOverridable);

		#endregion

		#region Voice

		public virtual float MinVoiceMixerVolume =>
			_voiceModule.MinMixerVolume;

		public virtual float MaxVoiceMixerVolume =>
			_voiceModule.MaxMixerVolume;

		public virtual float DefaultVoiceMixerVolume =>
			_voiceModule.DefaultMixerVolume;

		public virtual bool TryGetVoiceMixerVolume(out float volume) =>
			_voiceModule.TryGetMixerVolume(out volume);

		public virtual bool TryGetVoiceMixerVolume(string parameter, out float volume) =>
			_voiceModule.TryGetMixerVolume(parameter, out volume);

		public virtual bool TryGetVoiceExtraChannelMinMixerVolume(out float minVolume) =>
			_voiceModule.TryGetExtraChannelMinMixerVolume(out minVolume);

		public virtual bool TryGetVoiceExtraChannelMaxMixerVolume(out float maxVolume) =>
			_voiceModule.TryGetExtraChannelMaxMixerVolume(out maxVolume);

		public virtual bool TryGetVoiceExtraChannelDefaultMixerVolume(out float volume) =>
			_voiceModule.TryGetExtraChannelDefaultMixerVolume(out volume);

		public virtual bool TryGetVoiceExtraChannelMixerVolume(out float volume) =>
			_voiceModule.TryGetExtraChannelMixerVolume(out volume);

		public virtual bool TryGetVoiceChannelMinMixerVolume(string channelDataId, out float minVolume) =>
			_voiceModule.TryGetChannelMinMixerVolume(channelDataId, out minVolume);

		public virtual bool TryGetVoiceChannelMaxMixerVolume(string channelDataId, out float maxVolume) =>
			_voiceModule.TryGetChannelMaxMixerVolume(channelDataId, out maxVolume);

		public virtual bool TryGetVoiceChannelDefaultMixerVolume(string channelDataId, out float volume) =>
			_voiceModule.TryGetChannelDefaultMixerVolume(channelDataId, out volume);

		public virtual bool TryGetVoiceChannelMixerVolume(string channelDataId, out float volume) =>
			_voiceModule.TryGetChannelMixerVolume(channelDataId, out volume);


		public virtual bool CheckHasVoiceDataIdInConfig(string voiceDataId) =>
			_voiceModule.CheckIsAudioDataIdInConfig(voiceDataId);

		public virtual bool CheckHasVoice(string voiceId) =>
			_voiceModule.TryGetAudioReadModel(voiceId, out _);

		public virtual bool CheckIsVoicePlaying(string voiceId) =>
			_voiceModule.CheckIsPlaying(voiceId);

		public virtual bool CheckIsVoicePause(string voiceId) =>
			_voiceModule.CheckIsPause(voiceId);

		public virtual float GetVoiceDuration(string voiceDataId) =>
			_voiceModule.GetDuration(voiceDataId);

		public virtual bool TryGetVoiceReadModel(string voiceId, out IAudioReadModel voiceReadModel) =>
			_voiceModule.TryGetAudioReadModel(voiceId, out voiceReadModel);

		public virtual bool TryGetVoiceVolume(string voiceId, out float volume) =>
			_voiceModule.TryGetAudioVolume(voiceId, out volume);

		public virtual bool TryGetVoiceIsOverridable(string voiceId, out bool isOverridable) =>
			_voiceModule.TryGetAudioIsOverridable(voiceId, out isOverridable);

		#endregion


		[Preserve]
		public AudioPlayer(IAudioPlayerConfig audioPlayerConfig, IAssetProvider assetProvider, IVoiceAssetProvider voiceAssetProvider)
		{
			_audioPlayerConfig = audioPlayerConfig;

			_bgmModule = new AudioModule(audioPlayerConfig.BgmModuleConfig, assetProvider, assetProvider, audioPlayerConfig.AudioMixer, _audioPlayerConfig.IsDontDestroyOnLoad);
			_sfxModule = new AudioModule(audioPlayerConfig.SfxModuleConfig, assetProvider, assetProvider, audioPlayerConfig.AudioMixer, _audioPlayerConfig.IsDontDestroyOnLoad);
			_voiceModule = new AudioModule(audioPlayerConfig.VoiceModuleConfig, voiceAssetProvider, assetProvider, audioPlayerConfig.AudioMixer, _audioPlayerConfig.IsDontDestroyOnLoad);

			_bgmModule.OnSpawn += (callerId, bgmId, bgmDataId, userId, isOverridable, isRecord, _) => OnBgmSpawn?.Invoke(callerId, bgmId, bgmDataId, userId, isOverridable, isRecord);
			_bgmModule.OnStop += (callerId, bgmId, bgmDataId) => OnBgmStop?.Invoke(callerId, bgmId, bgmDataId);
			_bgmModule.OnComplete += (callerId, bgmId, bgmDataId) => OnBgmComplete?.Invoke(callerId, bgmId, bgmDataId);

			_sfxModule.OnSpawn += (callerId, sfxId, sfxDataId, userId, isOverridable, isRecord, _) => OnSfxSpawn?.Invoke(callerId, sfxId, sfxDataId, userId, isOverridable, isRecord);
			_sfxModule.OnStop += (callerId, sfxId, sfxDataId) => OnSfxStop?.Invoke(callerId, sfxId, sfxDataId);
			_sfxModule.OnComplete += (callerId, sfxId, sfxDataId) => OnSfxComplete?.Invoke(callerId, sfxId, sfxDataId);

			_voiceModule.OnSpawn += (callerId, voiceId, voiceDataId, userId, isOverridable, isRecord, channelDataId) => OnVoiceSpawn?.Invoke(callerId, voiceId, voiceDataId, userId, isOverridable, isRecord, channelDataId);
			_voiceModule.OnStop += (callerId, voiceId, voiceDataId) => OnVoiceStop?.Invoke(callerId, voiceId, voiceDataId);
			_voiceModule.OnComplete += (callerId, voiceId, voiceDataId) => OnVoiceComplete?.Invoke(callerId, voiceId, voiceDataId);

			voiceAssetProvider.OnChangedLocaleBeforeAsync += m_OnChangedLocaleBeforeAsync;
			voiceAssetProvider.OnChangedLocaleStartAsync += m_OnChangedLocaleStartAsync;

			Awaitable m_OnChangedLocaleBeforeAsync(string locale, AsyncToken asyncToken) =>
				OnChangedVoiceLocaleBeforeAsync?.Invoke(locale, asyncToken) ?? Async.Completed;

			Awaitable m_OnChangedLocaleStartAsync(string locale, AsyncToken asyncToken) =>
				OnChangedVoiceLocaleAsync?.Invoke(locale, asyncToken) ?? Async.Completed;
		}

		public virtual void Initialize(Transform rootParent = null)
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

			SetMasterMixerVolume(_audioPlayerConfig.DefaultMasterMixerVolume);

			OnInitialize?.Invoke();
		}

		public virtual void Release()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("[AudioPlayer::Release] AudioPlayer is not initialized.");
				return;
			}

			OnRelease?.Invoke();

			_bgmModule.Release();
			_sfxModule.Release();
			_voiceModule.Release();

			var rootGameObject = _root.gameObject;
			_root = null;
			ObjectUtils.DestroyOrImmediate(rootGameObject);
		}

		public virtual void Tick(float deltaTime)
		{
			if (!IsInitialized)
				return;

			_bgmModule.Tick(deltaTime);
			_sfxModule.Tick(deltaTime);
			_voiceModule.Tick(deltaTime);
		}

		public virtual void SetMasterMixerVolumeDefault() =>
			SetMasterMixerVolume(_audioPlayerConfig.DefaultMasterMixerVolume);

		public virtual void SetMasterMixerVolume(float volume)
		{
			if (_audioPlayerConfig.AudioMixer is null)
			{
				Debug.LogWarning("[AudioPlayer::SetMasterMixerVolume] AudioMixer is null.");
				return;
			}

			_audioPlayerConfig.AudioMixer.SetFloat(_audioPlayerConfig.MasterMixerParameter, m_GetLinearToLogarithmicScale(volume));

			float m_GetLinearToLogarithmicScale(float value) =>
				Mathf.Log(Mathf.Clamp(value, 0.001f, 1)) * 20.0f;
		}

		public virtual async Awaitable StopAllAsync(float fadeTime = 0, AsyncToken asyncToken = default)
		{
			var awaitables = new List<Awaitable>();
			awaitables.Add(StopAllBgmAsync(fadeTime, asyncToken));
			awaitables.Add(StopAllSfxAsync(fadeTime, asyncToken));
			awaitables.Add(StopAllVoiceAsync(fadeTime, asyncToken));

			await Async.AllAsync(awaitables);
		}

		#region Bgm

		public virtual void SetBgmMixerVolumeDefault() =>
			SetBgmMixerVolume(DefaultBgmMixerVolume);

		public virtual void SetBgmMixerVolume(float volume) =>
			_bgmModule.SetMixerVolume(volume);

		public virtual Awaitable LoadBgmAssetAsync(string bgmDataId, string errorMessage, AsyncToken asyncToken) =>
			_bgmModule.LoadAssetAsync(bgmDataId, errorMessage, asyncToken);

		public virtual void UnloadBgmAsset(string bgmDataId) =>
			_bgmModule.UnloadAsset(bgmDataId);


		public virtual string SpawnBgm(string bgmDataId, string userId, float volume = 1, bool isLoop = false, Transform parent = null, Vector3 position = default, bool isOverridable = false, bool isAutoDespawn = true, bool isRestrictContinuousPlay = true, bool isSyncTime = false, bool isRecord = false, string callerId = null) =>
			_bgmModule.Spawn(bgmDataId, userId, volume, isLoop, parent, position, isOverridable, isAutoDespawn, isRestrictContinuousPlay, isSyncTime, isRecord, string.Empty, callerId);

		public virtual string SpawnBgm(string bgmDataId, float volume = 1, bool isLoop = false, Transform parent = null, Vector3 position = default, bool isOverridable = false, bool isAutoDespawn = true, bool isRestrictContinuousPlay = true, bool isSyncTime = false, bool isRecord = false, string callerId = null) =>
			_bgmModule.Spawn(bgmDataId, string.Empty, volume, isLoop, parent, position, isOverridable, isAutoDespawn, isRestrictContinuousPlay, isSyncTime, isRecord, string.Empty, callerId);

		public virtual string SpawnBgm(bool isCustomBgmId, string bgmId, string bgmDataId, string userId, float volume = 1, bool isLoop = false, Transform parent = null, Vector3 position = default, bool isOverridable = false, bool isAutoDespawn = true, bool isRestrictContinuousPlay = true, bool isSyncTime = false, bool isRecord = false, string callerId = null) =>
			_bgmModule.Spawn(isCustomBgmId, bgmId, bgmDataId, userId, volume, isLoop, parent, position, isOverridable, isAutoDespawn, isRestrictContinuousPlay, isSyncTime, isRecord, string.Empty, callerId);

		public virtual Awaitable<string> PlayBgmAsync(string bgmDataId, float volume = 1, float fadeTime = 0, bool isLoop = false, Vector3 position = default, bool isOverridable = false, bool isAutoDespawn = true, bool isRestrictContinuousPlay = true, bool isSyncTime = false, bool isRecord = false, string callerId = null, AsyncToken asyncToken = default) =>
			_bgmModule.PlayAsync(bgmDataId, volume, fadeTime, isLoop, null, position, isOverridable, isAutoDespawn, isRestrictContinuousPlay, isSyncTime, isRecord, string.Empty, callerId, asyncToken);

		public virtual Awaitable<string> PlayBgmAsync(string bgmDataId, string userId, float volume = 1, float fadeTime = 0, bool isLoop = false, Transform parent = null, Vector3 position = default, bool isOverridable = false, bool isAutoDespawn = true, bool isRestrictContinuousPlay = true, bool isSyncTime = false, bool isRecord = false, string callerId = null, AsyncToken asyncToken = default) =>
			_bgmModule.PlayAsync(bgmDataId, userId, volume, fadeTime, isLoop, parent, position, isOverridable, isAutoDespawn, isRestrictContinuousPlay, isSyncTime, isRecord, string.Empty, callerId, asyncToken);

		public virtual Awaitable<string> PlayBgmAsync(string bgmDataId, float volume = 1, float fadeTime = 0, bool isLoop = false, Transform parent = null, Vector3 position = default, bool isOverridable = false, bool isAutoDespawn = true, bool isRestrictContinuousPlay = true, bool isSyncTime = false, bool isRecord = false, string callerId = null, AsyncToken asyncToken = default) =>
			_bgmModule.PlayAsync(bgmDataId, volume, fadeTime, isLoop, parent, position, isOverridable, isAutoDespawn, isRestrictContinuousPlay, isSyncTime, isRecord, string.Empty, callerId, asyncToken);

		public virtual Awaitable RestartBgmAsync(string bgmId, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_bgmModule.RestartAsync(bgmId, fadeTime, asyncToken);


		public virtual Awaitable ModifyBgmAsync(string bgmId, float volume, bool isLoop, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_bgmModule.ModifyAsync(bgmId, volume, isLoop, fadeTime, asyncToken);

		public virtual Awaitable ModifyBgmAsync(string bgmId, float volume, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_bgmModule.ModifyAsync(bgmId, volume, fadeTime, asyncToken);

		public virtual void SetBgmTime(string bgmId, float time, bool isSyncTime) =>
			_bgmModule.SetTime(bgmId, time, isSyncTime);

		public virtual Awaitable PauseBgmAsync(string bgmId, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_bgmModule.PauseAsync(bgmId, fadeTime, asyncToken);

		public virtual Awaitable ResumeBgmAsync(string bgmId, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_bgmModule.ResumeAsync(bgmId, fadeTime, asyncToken);


		public virtual Awaitable StopBgmAsync(string bgmId, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_bgmModule.StopAsync(bgmId, fadeTime, asyncToken);

		public virtual Awaitable StopAllBgmAsync(float fadeTime = 0, AsyncToken asyncToken = default) =>
			_bgmModule.StopAllAsync(fadeTime, asyncToken);

		#endregion

		#region Sfx

		public virtual void SetSfxMixerVolumeDefault() =>
			SetSfxMixerVolume(DefaultSfxMixerVolume);

		public virtual void SetSfxMixerVolume(float volume) =>
			_sfxModule.SetMixerVolume(volume);

		public virtual Awaitable LoadSfxAssetAsync(string sfxDataId, string errorMessage, AsyncToken asyncToken) =>
			_sfxModule.LoadAssetAsync(sfxDataId, errorMessage, asyncToken);

		public virtual void UnloadSfxAsset(string sfxDataId) =>
			_sfxModule.UnloadAsset(sfxDataId);

		public virtual string SpawnSfx(string sfxDataId, string userId, float volume = 1, bool isLoop = false, Transform parent = null, Vector3 position = default, bool isOverridable = false, bool isAutoDespawn = true, bool isRestrictContinuousPlay = true, bool isSyncTime = false, bool isRecord = false, string callerId = null) =>
			_sfxModule.Spawn(sfxDataId, userId, volume, isLoop, parent, position, isOverridable, isAutoDespawn, isRestrictContinuousPlay, isSyncTime, isRecord, string.Empty, callerId);

		public virtual string SpawnSfx(string sfxDataId, float volume = 1, bool isLoop = false, Transform parent = null, Vector3 position = default, bool isOverridable = false, bool isAutoDespawn = true, bool isRestrictContinuousPlay = true, bool isSyncTime = false, bool isRecord = false, string callerId = null) =>
			_sfxModule.Spawn(sfxDataId, string.Empty, volume, isLoop, parent, position, isOverridable, isAutoDespawn, isRestrictContinuousPlay, isSyncTime, isRecord, string.Empty, callerId);

		public virtual string SpawnSfx(bool isCustomSfxId, string sfxId, string sfxDataId, string userId, float volume = 1, bool isLoop = false, Transform parent = null, Vector3 position = default, bool isOverridable = false, bool isAutoDespawn = true, bool isRestrictContinuousPlay = true, bool isSyncTime = false, bool isRecord = false, string callerId = null) =>
			_sfxModule.Spawn(isCustomSfxId, sfxId, sfxDataId, userId, volume, isLoop, parent, position, isOverridable, isAutoDespawn, isRestrictContinuousPlay, isSyncTime, isRecord, string.Empty, callerId);

		public virtual void DespawnSfx(string sfxId) =>
			_sfxModule.Despawn(sfxId);


		public virtual Awaitable<string> PlaySfxAsync(string sfxDataId, string userId, float volume = 1, float fadeTime = 0, bool isLoop = false, Transform parent = null, Vector3 position = default, bool isOverridable = false, bool isAutoDespawn = true, bool isRestrictContinuousPlay = true, bool isSyncTime = false, bool isRecord = false, string callerId = null, AsyncToken asyncToken = default) =>
			_sfxModule.PlayAsync(sfxDataId, userId, volume, fadeTime, isLoop, parent, position, isOverridable, isAutoDespawn, isRestrictContinuousPlay, isSyncTime, isRecord, string.Empty, callerId, asyncToken);

		public virtual Awaitable<string> PlaySfxAsync(string sfxDataId, float volume = 1, float fadeTime = 0, bool isLoop = false, Vector3 position = default, bool isOverridable = false, bool isAutoDespawn = true, bool isRestrictContinuousPlay = true, bool isSyncTime = false, bool isRecord = false, string callerId = null, AsyncToken asyncToken = default) =>
			_sfxModule.PlayAsync(sfxDataId, volume, fadeTime, isLoop, null, position, isOverridable, isAutoDespawn, isRestrictContinuousPlay, isSyncTime, isRecord, string.Empty, callerId, asyncToken);

		public virtual Awaitable<string> PlaySfxAsync(string sfxDataId, float volume = 1, float fadeTime = 0, bool isLoop = false, Transform parent = null, Vector3 position = default, bool isOverridable = false, bool isAutoDespawn = true, bool isRestrictContinuousPlay = true, bool isSyncTime = false, bool isRecord = false, string callerId = null, AsyncToken asyncToken = default) =>
			_sfxModule.PlayAsync(sfxDataId, volume, fadeTime, isLoop, parent, position, isOverridable, isAutoDespawn, isRestrictContinuousPlay, isSyncTime, isRecord, string.Empty, callerId, asyncToken);

		public virtual Awaitable RestartSfxAsync(string sfxId, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_sfxModule.RestartAsync(sfxId, fadeTime, asyncToken);

		public virtual Awaitable ModifySfxAsync(string sfxId, float volume, bool isLoop, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_sfxModule.ModifyAsync(sfxId, volume, isLoop, fadeTime, asyncToken);

		public virtual Awaitable ModifySfxAsync(string sfxId, float volume, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_sfxModule.ModifyAsync(sfxId, volume, fadeTime, asyncToken);

		public virtual void SetSfxTime(string sfxId, float time, bool isSyncTime) =>
			_sfxModule.SetTime(sfxId, time, isSyncTime);

		public virtual Awaitable ResumeSfxAsync(string sfxId, float time, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_sfxModule.ResumeAsync(sfxId, time, fadeTime, asyncToken);

		public virtual Awaitable ResumeSfxAsync(string sfxId, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_sfxModule.ResumeAsync(sfxId, fadeTime, asyncToken);

		public virtual Awaitable PauseSfxAsync(string sfxId, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_sfxModule.PauseAsync(sfxId, fadeTime, asyncToken);

		public virtual Awaitable StopSfxAsync(string sfxId, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_sfxModule.StopAsync(sfxId, fadeTime, asyncToken);

		public virtual Awaitable StopAllSfxAsync(float fadeTime = 0, AsyncToken asyncToken = default) =>
			_sfxModule.StopAllAsync(fadeTime, asyncToken);

		#endregion

		#region Voice

		public virtual void SetVoiceMixerVolumeDefault() =>
			SetVoiceMixerVolume(DefaultVoiceMixerVolume);

		public virtual void SetVoiceMixerVolume(float volume) =>
			_voiceModule.SetMixerVolume(volume);

		public virtual void SetVoiceMixerVolume(string volumeParameter, float volume) =>
			_voiceModule.SetMixerVolume(volumeParameter, volume);

		public virtual void SetVoiceExtraChannelMixerVolumeDefault()
		{
			if (!_voiceModule.TryGetExtraChannelDefaultMixerVolume(out var volume))
				return;

			SetVoiceExtraChannelMixerVolume(volume);
		}

		public virtual void SetVoiceExtraChannelMixerVolume(float volume) =>
			_voiceModule.SetExtraChannelMixerVolume(volume);

		public virtual void SetAllVoiceChannelMixerVolumeDefault()
		{
			foreach (var channelDataId in AllVoiceChannelDataIds)
				SetVoiceChannelMixerVolumeDefault(channelDataId);
		}

		public virtual void SetVoiceChannelMixerVolumeDefault(string channelDataId)
		{
			if (!_voiceModule.TryGetChannelDefaultMixerVolume(channelDataId, out var volume))
				return;

			SetVoiceChannelMixerVolume(channelDataId, volume);
		}

		public virtual void SetVoiceChannelMixerVolume(string channelDataId, float volume) =>
			_voiceModule.SetChannelMixerVolume(channelDataId, volume);


		public virtual Awaitable LoadVoiceAssetAsync(string voiceDataId, string errorMessage, AsyncToken asyncToken = default) =>
			_voiceModule.LoadAssetAsync(voiceDataId, errorMessage, asyncToken);

		public virtual void UnloadVoiceAsset(string voiceDataId) =>
			_voiceModule.UnloadAsset(voiceDataId);

		public virtual string SpawnVoice(string voiceDataId, string userId, float volume = 1, bool isLoop = false, Transform parent = null, Vector3 position = default, bool isOverridable = false, bool isAutoDespawn = true, bool isRestrictContinuousPlay = true, bool isSyncTime = false, bool isRecord = false, string channelDataId = null, string callerId = null) =>
			_voiceModule.Spawn(voiceDataId, userId, volume, isLoop, parent, position, isOverridable, isAutoDespawn, isRestrictContinuousPlay, isSyncTime, isRecord, channelDataId, callerId);

		public virtual string SpawnVoice(string voiceDataId, float volume = 1, bool isLoop = false, Transform parent = null, Vector3 position = default, bool isOverridable = false, bool isAutoDespawn = true, bool isRestrictContinuousPlay = true, bool isSyncTime = false, bool isRecord = false, string channelDataId = null, string callerId = null) =>
			_voiceModule.Spawn(voiceDataId, string.Empty, volume, isLoop, parent, position, isOverridable, isAutoDespawn, isRestrictContinuousPlay, isSyncTime, isRecord, channelDataId, callerId);

		public virtual string SpawnVoice(bool isCustomVoiceId, string voiceId, string voiceDataId, string userId, float volume = 1, bool isLoop = false, Transform parent = null, Vector3 position = default, bool isOverridable = false, bool isAutoDespawn = true, bool isRestrictContinuousPlay = true, bool isSyncTime = false, bool isRecord = false, string channelDataId = null, string callerId = null) =>
			_voiceModule.Spawn(isCustomVoiceId, voiceId, voiceDataId, userId, volume, isLoop, parent, position, isOverridable, isAutoDespawn, isRestrictContinuousPlay, isSyncTime, isRecord, channelDataId, callerId);


		public virtual void DespawnVoice(string voiceId) =>
			_voiceModule.Despawn(voiceId);


		public virtual Awaitable<string> PlayVoiceAsync(string voiceDataId, string userId, float volume = 1, float fadeTime = 0, bool isLoop = false, Transform parent = null, Vector3 position = default, bool isOverridable = false, bool isAutoDespawn = true, bool isRestrictContinuousPlay = true, bool isSyncTime = false, bool isRecord = false, string channelDataId = null, string callerId = null, AsyncToken asyncToken = default) =>
			_voiceModule.PlayAsync(voiceDataId, userId, volume, fadeTime, isLoop, parent, position, isOverridable, isAutoDespawn, isRestrictContinuousPlay, isSyncTime, isRecord, channelDataId, callerId, asyncToken);

		public virtual Awaitable<string> PlayVoiceAsync(string voiceDataId, float volume = 1, float fadeTime = 0, bool isLoop = false, Transform parent = null, Vector3 position = default, bool isOverridable = false, bool isAutoDespawn = true, bool isRestrictContinuousPlay = true, bool isSyncTime = false, bool isRecord = false, string channelDataId = null, string callerId = null, AsyncToken asyncToken = default) =>
			_voiceModule.PlayAsync(voiceDataId, volume, fadeTime, isLoop, parent, position, isOverridable, isAutoDespawn, isRestrictContinuousPlay, isSyncTime, isRecord, channelDataId, callerId, asyncToken);

		public virtual Awaitable RestartVoiceAsync(string voiceId, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_voiceModule.RestartAsync(voiceId, fadeTime, asyncToken);

		public virtual Awaitable ModifyVoiceAsync(string voiceId, float volume, bool isLoop, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_voiceModule.ModifyAsync(voiceId, volume, isLoop, fadeTime, asyncToken);

		public virtual Awaitable ModifyVoiceAsync(string voiceId, float volume, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_voiceModule.ModifyAsync(voiceId, volume, fadeTime, asyncToken);

		public virtual void SetVoiceTime(string voiceId, float time, bool isSyncTime) =>
			_voiceModule.SetTime(voiceId, time, isSyncTime);

		public virtual Awaitable PauseVoiceAsync(string voiceId, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_voiceModule.PauseAsync(voiceId, fadeTime, asyncToken);

		public virtual Awaitable ResumeVoiceAsync(string voiceId, float time, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_voiceModule.ResumeAsync(voiceId, time, fadeTime, asyncToken);

		public virtual Awaitable ResumeVoiceAsync(string voiceId, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_voiceModule.ResumeAsync(voiceId, fadeTime, asyncToken);

		public virtual Awaitable StopVoiceAsync(string voiceId, float fadeTime = 0, AsyncToken asyncToken = default) =>
			_voiceModule.StopAsync(voiceId, fadeTime, asyncToken);

		public virtual Awaitable StopAllVoiceAsync(float fadeTime = 0, AsyncToken asyncToken = default) =>
			_voiceModule.StopAllAsync(fadeTime, asyncToken);

		#endregion
	}
}