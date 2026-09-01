using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using UnityEngine.Scripting;

namespace CizaAudioModule.Implement
{
	[CreateAssetMenu(fileName = "Aud.AudioPlayerConfig.asset", menuName = "Ciza/AudioModule/AudioPlayerConfig", order = 100)]
	public class AudioPlayerConfig : ScriptableObject, IAudioPlayerConfig
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected string _rootName;

		[SerializeField]
		protected bool _isDontDestroyOnLoad;

		[Space]
		[SerializeField]
		protected AudioMixer _audioMixer;

		[SerializeField]
		protected string _masterMixerGroupPath;

		[SerializeField]
		protected string _masterMixerParameter;

		[Range(0, 1)]
		[SerializeField]
		protected float _defaultMasterVolume;

		[Space]
		[SerializeField]
		protected AudioModuleConfig _bgmModuleConfig;

		[SerializeField]
		protected AudioModuleConfig _sfxModuleConfig;

		[SerializeField]
		protected VoiceAudioModuleConfig _voiceModuleConfig;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string RootName => _rootName;
		public virtual bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

		// Start 之後才會初始好，使用時請注意。
		public virtual AudioMixer AudioMixer => _audioMixer;
		public virtual string MasterMixerGroupPath => _masterMixerGroupPath;
		public virtual string MasterMixerParameter => _masterMixerParameter;
		public virtual float DefaultMasterVolume => _defaultMasterVolume;

		public virtual IAudioModuleConfig BgmModuleConfig => _bgmModuleConfig;
		public virtual IAudioModuleConfig SfxModuleConfig => _sfxModuleConfig;
		public virtual IAudioModuleConfig VoiceModuleConfig => _voiceModuleConfig;


		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_rootName = "[AudioPlayer]";
			_isDontDestroyOnLoad = true;

			_audioMixer = null;

			_masterMixerGroupPath = "Master";
			_masterMixerParameter = "Master";
			_defaultMasterVolume = 0.7f;

			_bgmModuleConfig = new AudioModuleConfig("Bgm", "Master/Bgm", "Bgm", "Aud.BgmAudio");
			_sfxModuleConfig = new AudioModuleConfig("Sfx", "Master/Sfx", "Sfx", "Aud.SfxAudio");
			_voiceModuleConfig = new VoiceAudioModuleConfig("Voice", "Master/Voice", "Voice", "Master/Voice/Extra", "Extra", "Aud.VoiceAudio");
		}

		[Serializable]
		public class AudioModuleConfig : IAudioModuleConfig, IZomeraphyPanel
		{
			// VARIABLE: -----------------------------------------------------------------------------

			[SerializeField]
			protected string _poolRootName;

			[Space]
			[SerializeField]
			protected string _poolPrefix;

			[SerializeField]
			protected string _poolSuffix;

			[Space]
			[Space]
			[SerializeField]
			protected string _audioMixerGroupPath;

			[SerializeField]
			protected string _audioMixerVolumeParameter;

			[Range(0, 1)]
			[SerializeField]
			protected float _defaultVolume;

			[Space]
			[Space]
			[SerializeField]
			protected RestrictContinuousPlayEnabler _hasRestrictContinuousPlay;

			[Space]
			[SerializeField]
			protected string _prefabAddress;

			[SerializeField]
			protected AudioInfoMapList _infoMapList;

			// PUBLIC VARIABLE: ---------------------------------------------------------------------

			public virtual string PoolRootName => _poolRootName;

			public virtual string PoolPrefix => _poolPrefix;
			public virtual string PoolSuffix => _poolSuffix;

			public virtual string AudioMixerGroupPath => _audioMixerGroupPath;
			public virtual string AudioMixerVolumeParameter => _audioMixerVolumeParameter;
			public virtual float DefaultVolume => _defaultVolume;


			public virtual bool TryGetExtraChannelInfo(out IAudioChannelInfo extraChannelInfo)
			{
				extraChannelInfo = null;
				return false;
			}

			public virtual string[] ChannelDataIds => Array.Empty<string>();

			public virtual bool TryGetChannelInfo(string channelDataId, out IAudioChannelInfo channelInfo)
			{
				channelInfo = null;
				return false;
			}

			public virtual bool TryGetRestrictContinuousPlay(out IRestrictContinuousPlay restrictContinuousPlay) =>
				_hasRestrictContinuousPlay.TryGetValue(out restrictContinuousPlay);


			public virtual string PrefabAddress => _prefabAddress;

			public virtual IReadOnlyDictionary<string, IAudioInfo> CreateAudioInfoMapByDataId()
			{
				Assert.IsNotNull(_infoMapList, "[AudioPlayerModuleConfig::CreateAudioInfoMapDataId] AudioInfos is null.");
				return _infoMapList.ToDictionary<IAudioInfo>();
			}

			// CONSTRUCTOR: ------------------------------------------------------------------------

			[Preserve]
			public AudioModuleConfig() : this(string.Empty, string.Empty, string.Empty, string.Empty) { }

			[Preserve]
			public AudioModuleConfig(string poolRootName, string audioMixerGroupPath, string audioMixerVolumeParameter, string defaultPrefabAddress) : this(poolRootName, string.Empty, "s", audioMixerGroupPath, audioMixerVolumeParameter, 0.7f, new RestrictContinuousPlayEnabler(), defaultPrefabAddress, new AudioInfoMapList()) { }

			[Preserve]
			public AudioModuleConfig(string poolRootName, string poolPrefix, string poolSuffix, string audioMixerGroupPath, string audioMixerVolumeParameter, float defaultVolume, RestrictContinuousPlayEnabler hasRestrictContinuousPlay, string prefabAddress, AudioInfoMapList infoMapList)
			{
				_poolRootName = poolRootName;

				_poolPrefix = poolPrefix;
				_poolSuffix = poolSuffix;

				_audioMixerGroupPath = audioMixerGroupPath;
				_audioMixerVolumeParameter = audioMixerVolumeParameter;
				_defaultVolume = defaultVolume;

				_hasRestrictContinuousPlay = hasRestrictContinuousPlay;

				_prefabAddress = prefabAddress;
				_infoMapList = infoMapList;
			}
		}


		[Serializable]
		public class VoiceAudioModuleConfig : IAudioModuleConfig, IZomeraphyPanel
		{
			// VARIABLE: -----------------------------------------------------------------------------

			[SerializeField]
			protected string _poolRootName;

			[Space]
			[SerializeField]
			protected string _poolPrefix;

			[SerializeField]
			protected string _poolSuffix;

			[Space]
			[Space]
			[SerializeField]
			protected string _audioMixerGroupPath;

			[SerializeField]
			protected string _audioMixerVolumeParameter;

			[Range(0, 1)]
			[SerializeField]
			protected float _defaultVolume;

			[Space]
			[SerializeField]
			protected MultipleAudioChannelInfoEnabler _hasMultipleChannelInfo;

			[Space]
			[Space]
			[SerializeField]
			protected RestrictContinuousPlayEnabler _hasRestrictContinuousPlay;

			[Space]
			[SerializeField]
			protected string _prefabAddress;

			[SerializeField]
			protected AudioInfoMapList _infoMapList;

			// PUBLIC VARIABLE: ---------------------------------------------------------------------

			public virtual string PoolRootName => _poolRootName;

			public virtual string PoolPrefix => _poolPrefix;
			public virtual string PoolSuffix => _poolSuffix;

			public virtual string AudioMixerGroupPath => _audioMixerGroupPath;
			public virtual string AudioMixerVolumeParameter => _audioMixerVolumeParameter;
			public virtual float DefaultVolume => _defaultVolume;


			public virtual bool TryGetExtraChannelInfo(out IAudioChannelInfo extraChannelInfo)
			{
				if (!_hasMultipleChannelInfo.TryGetValue(out var multipleChannelInfo))
				{
					extraChannelInfo = null;
					return false;
				}

				extraChannelInfo = multipleChannelInfo.ExtraChannelInfo;
				return true;
			}

			public virtual string[] ChannelDataIds => _hasMultipleChannelInfo.TryGetValue(out var multipleChannelInfo) ? multipleChannelInfo.ChannelDataIds : Array.Empty<string>();

			public virtual bool TryGetChannelInfo(string channelDataId, out IAudioChannelInfo channelInfo)
			{
				if (!_hasMultipleChannelInfo.TryGetValue(out var multipleChannelInfo))
				{
					channelInfo = null;
					return false;
				}

				return multipleChannelInfo.TryGetChannelInfo(channelDataId, out channelInfo);
			}

			public virtual bool TryGetRestrictContinuousPlay(out IRestrictContinuousPlay restrictContinuousPlay) =>
				_hasRestrictContinuousPlay.TryGetValue(out restrictContinuousPlay);


			public virtual string PrefabAddress => _prefabAddress;

			public virtual IReadOnlyDictionary<string, IAudioInfo> CreateAudioInfoMapByDataId()
			{
				Assert.IsNotNull(_infoMapList, "[AudioPlayerModuleConfig::CreateAudioInfoMapDataId] AudioInfos is null.");
				return _infoMapList.ToDictionary<IAudioInfo>();
			}

			// CONSTRUCTOR: ------------------------------------------------------------------------

			[Preserve]
			public VoiceAudioModuleConfig() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty) { }

			[Preserve]
			public VoiceAudioModuleConfig(string poolRootName, string audioMixerGroupPath, string audioMixerVolumeParameter, string extraChannelAudioMixerGroupPath, string extraChannelAudioMixerVolumeParameter, string defaultPrefabAddress) : this(poolRootName, string.Empty, "s", audioMixerGroupPath, audioMixerVolumeParameter, 0.7f, extraChannelAudioMixerGroupPath, extraChannelAudioMixerVolumeParameter, 1, new RestrictContinuousPlayEnabler(), defaultPrefabAddress, new AudioInfoMapList()) { }

			[Preserve]
			public VoiceAudioModuleConfig(string poolRootName, string poolPrefix, string poolSuffix, string audioMixerGroupPath, string audioMixerVolumeParameter, float defaultVolume, string extraChannelAudioMixerGroupPath, string extraChannelAudioMixerVolumeParameter, float extraChannelDefaultVolume, RestrictContinuousPlayEnabler hasRestrictContinuousPlay, string prefabAddress, AudioInfoMapList infoMapList)
			{
				_poolRootName = poolRootName;

				_poolPrefix = poolPrefix;
				_poolSuffix = poolSuffix;

				_audioMixerGroupPath = audioMixerGroupPath;
				_audioMixerVolumeParameter = audioMixerVolumeParameter;
				_defaultVolume = defaultVolume;

				_hasMultipleChannelInfo = new MultipleAudioChannelInfoEnabler(new MultipleAudioChannelInfo(new AudioChannelInfo(extraChannelAudioMixerGroupPath, extraChannelAudioMixerVolumeParameter, extraChannelDefaultVolume), new AudioChannelInfoMapList()));

				_hasRestrictContinuousPlay = hasRestrictContinuousPlay;

				_prefabAddress = prefabAddress;
				_infoMapList = infoMapList;
			}
		}
	}
}