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
		private string _rootName = "[AudioPlayer]";

		[SerializeField]
		private bool _isDontDestroyOnLoad = true;

		[Space]
		[SerializeField]
		private AudioMixer _audioMixer;

		[SerializeField]
		private string _masterMixerGroupPath = "Master";

		[SerializeField]
		private string _masterMixerParameter = "Master";

		[Range(0, 1)]
		[SerializeField]
		private float _defaultMasterVolume = 0.7f;

		[Space]
		[SerializeField]
		private AudioModuleConfig _bgmModuleConfig = new AudioModuleConfig("Bgm", "Master/Bgm", "Bgm", "BgmAudio");

		[SerializeField]
		private AudioModuleConfig _sfxModuleConfig = new AudioModuleConfig("Sfx", "Master/Sfx", "Sfx", "SfxAudio");

		[SerializeField]
		private AudioModuleConfig _voiceModuleConfig = new AudioModuleConfig("Voice", "Master/Voice", "Voice", "VoiceAudio");

		public string RootName => _rootName;
		public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

		public AudioMixer AudioMixer => _audioMixer;
		public string MasterMixerGroupPath => _masterMixerGroupPath;
		public string MasterMixerParameter => _masterMixerParameter;
		public float DefaultMasterVolume => _defaultMasterVolume;

		public IAudioModuleConfig BgmModuleConfig => _bgmModuleConfig;
		public IAudioModuleConfig SfxModuleConfig => _sfxModuleConfig;
		public IAudioModuleConfig VoiceModuleConfig => _voiceModuleConfig;

		[Serializable]
		private class AudioModuleConfig : IAudioModuleConfig
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
			protected string _defaultPrefabAddress;

			[SerializeField]
			protected AudioInfo[] _audioInfos;

			[Preserve]
			public AudioModuleConfig() { }

			[Preserve]
			public AudioModuleConfig(string poolRootName, string audioMixerGroupPath, string audioMixerParameter, string defaultPrefabAddress)
			{
				_poolRootName = poolRootName;
				_audioMixerGroupPath = audioMixerGroupPath;
				_audioMixerParameter = audioMixerParameter;
				_defaultPrefabAddress = defaultPrefabAddress;
			}

			public virtual string PoolRootName => _poolRootName;

			public virtual string PoolPrefix => _poolPrefix;
			public virtual string PoolSuffix => _poolSuffix;

			public virtual string AudioMixerGroupPath => _audioMixerGroupPath;
			public virtual string AudioMixerParameter => _audioMixerParameter;
			public virtual float DefaultVolume => _defaultVolume;

			public virtual bool TryGetRestrictContinuousPlay(out IRestrictContinuousPlay restrictContinuousPlay) =>
				_hasRestrictContinuousPlay.TryGetValue(out restrictContinuousPlay);


			public virtual string DefaultPrefabAddress => _defaultPrefabAddress;

			public virtual IReadOnlyDictionary<string, IAudioInfo> CreateAudioInfoMapDataId()
			{
				Assert.IsNotNull(_audioInfos, "[AudioPlayerModuleConfig::CreateAudioInfoMapDataId] AudioInfos is null.");

				var audioInfoMap = new Dictionary<string, IAudioInfo>();

				if (_audioInfos is null)
					return audioInfoMap;

				foreach (var audioInfo in _audioInfos)
					audioInfoMap.Add(audioInfo.DataId, audioInfo);

				return audioInfoMap;
			}

			[Serializable]
			public class AudioInfo : IAudioInfo
			{
				[SerializeField]
				private string _dataId;

				[Space]
				[SerializeField]
				private string _clipAddress;

				[SerializeField]
				private string _prefabAddress;

				public string DataId => _dataId;

				public string ClipAddress => _clipAddress;
				public string PrefabAddress => _prefabAddress;
			}
		}
	}
}