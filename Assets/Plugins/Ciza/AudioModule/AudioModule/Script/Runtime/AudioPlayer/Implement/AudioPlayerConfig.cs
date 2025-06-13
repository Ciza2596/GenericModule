using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using UnityEngine.Scripting;

namespace CizaAudioModule.Implement
{
	[CreateAssetMenu(fileName = "AudioPlayerConfig", menuName = "Ciza/AudioModule/AudioPlayerConfig", order = 100)]
	public class AudioPlayerConfig : ScriptableObject, IAudioPlayerConfig
	{
		[SerializeField]
		protected string _rootName = "[AudioPlayer]";

		[SerializeField]
		protected bool _isDontDestroyOnLoad = true;

		[Space]
		[SerializeField]
		protected AudioMixer _audioMixer;

		[SerializeField]
		protected string _masterMixerGroupPath = "Master";

		[SerializeField]
		protected string _masterMixerParameter = "Master";

		[Range(0, 1)]
		[SerializeField]
		protected float _defaultMasterVolume = 0.7f;

		[Space]
		[SerializeField]
		protected AudioModuleConfig _bgmModuleConfig = new AudioModuleConfig("Bgm", "Master/Bgm", "Bgm", "BgmAudio");

		[SerializeField]
		protected AudioModuleConfig _sfxModuleConfig = new AudioModuleConfig("Sfx", "Master/Sfx", "Sfx", "SfxAudio");

		[SerializeField]
		protected AudioModuleConfig _voiceModuleConfig = new AudioModuleConfig("Voice", "Master/Voice", "Voice", "VoiceAudio");

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

		[Serializable]
		public class AudioModuleConfig : IAudioModuleConfig, IZomeraphyPanel
		{
			[SerializeField]
			protected string _poolRootName;

			[Space]
			[SerializeField]
			protected string _poolPrefix;

			[SerializeField]
			protected string _poolSuffix = "s";

			[Space]
			[SerializeField]
			protected string _audioMixerGroupPath;

			[SerializeField]
			protected string _audioMixerParameter;

			[Range(0, 1)]
			[SerializeField]
			protected float _defaultVolume = 0.7f;

			[Space]
			[SerializeField]
			protected RestrictContinuousPlayEnabler _hasRestrictContinuousPlay;

			[Space]
			[SerializeField]
			protected string _prefabAddress;

			[SerializeField]
			protected AudioInfoMapList _infoMapList;

			[Preserve]
			public AudioModuleConfig() { }

			[Preserve]
			public AudioModuleConfig(string poolRootName, string audioMixerGroupPath, string audioMixerParameter, string defaultPrefabAddress)
			{
				_poolRootName = poolRootName;
				_audioMixerGroupPath = audioMixerGroupPath;
				_audioMixerParameter = audioMixerParameter;
				_prefabAddress = defaultPrefabAddress;
			}

			public virtual string PoolRootName => _poolRootName;

			public virtual string PoolPrefix => _poolPrefix;
			public virtual string PoolSuffix => _poolSuffix;

			public virtual string AudioMixerGroupPath => _audioMixerGroupPath;
			public virtual string AudioMixerParameter => _audioMixerParameter;
			public virtual float DefaultVolume => _defaultVolume;

			public virtual bool TryGetRestrictContinuousPlay(out IRestrictContinuousPlay restrictContinuousPlay) =>
				_hasRestrictContinuousPlay.TryGetValue(out restrictContinuousPlay);


			public virtual string PrefabAddress => _prefabAddress;

			public virtual IReadOnlyDictionary<string, IAudioInfo> CreateAudioInfoMapByDataId()
			{
				Assert.IsNotNull(_infoMapList, "[AudioPlayerModuleConfig::CreateAudioInfoMapDataId] AudioInfos is null.");
				return _infoMapList.ToDictionary<IAudioInfo>();
			}
		}
	}
}