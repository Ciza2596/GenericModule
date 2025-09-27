using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaAudioModule.Implement
{
	[CreateAssetMenu(fileName = "AudioModuleConfig", menuName = "Ciza/AudioModule/AudioModuleConfig")]
	public class AudioModuleConfig : ScriptableObject, IAudioModuleConfig
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
		[SerializeField]
		protected string _audioMixerGroupPath;

		[SerializeField]
		protected string _audioMixerParameter;

		[Range(0, 1)]
		[SerializeField]
		protected float _defaultVolume;

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
		public virtual string AudioMixerParameter => _audioMixerParameter;
		public virtual float DefaultVolume => _defaultVolume;

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

			_audioMixerGroupPath = "Master";
			_audioMixerParameter = "Master";
			_defaultVolume = 0.7f;

			_hasRestrictContinuousPlay = new RestrictContinuousPlayEnabler();

			_prefabAddress = string.Empty;

			_infoMapList = new AudioInfoMapList();
		}
	}
}