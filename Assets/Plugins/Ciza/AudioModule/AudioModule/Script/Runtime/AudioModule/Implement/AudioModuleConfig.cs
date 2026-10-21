using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaAudioModule.Implement
{
	[CreateAssetMenu(fileName = "Aud.Config.asset", menuName = "Ciza/AudioModule/Config")]
	public class AudioModuleConfig : ScriptableObject, IAudioModuleConfig
	{
		// CONST & STATIC: -----------------------------------------------------------------------

		public const float MIN_MIXER_VOLUME = 0f;
		public const float MAX_MIXER_VOLUME = 1f;

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
		protected string _mixerGroupPath;

		[SerializeField]
		protected string _mixerVolumeParameter;

		[Range(0, 1)]
		[SerializeField]
		protected float _defaultMixerVolume;

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

		public virtual string MixerGroupPath => _mixerGroupPath;
		public virtual string MixerVolumeParameter => _mixerVolumeParameter;

		public virtual float MinMixerVolume => MIN_MIXER_VOLUME;
		public virtual float MaxMixerVolume => MAX_MIXER_VOLUME;
		public virtual float DefaultMixerVolume => _defaultMixerVolume;


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
			Assert.IsNotNull(_infoMapList, "[AudioModuleConfig::CreateAudioInfoMapByDataId] AudioInfos is null.");

			var audioInfoMap = new Dictionary<string, IAudioInfo>();

			if (_infoMapList is null)
				return audioInfoMap;

			foreach (var pair in _infoMapList.KeyValuePairs)
				audioInfoMap.Add(pair.Value.DataId, pair.Value);

			return audioInfoMap;
		}

		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_poolRootName = "[AudioModule]";

			_poolPrefix = "";
			_poolSuffix = "s";

			_mixerGroupPath = "Master";
			_mixerVolumeParameter = "Master";
			_defaultMixerVolume = 0.7f;

			_hasMultipleChannelInfo = new MultipleAudioChannelInfoEnabler();

			_hasRestrictContinuousPlay = new RestrictContinuousPlayEnabler();

			_prefabAddress = string.Empty;

			_infoMapList = new AudioInfoMapList();
		}
	}
}