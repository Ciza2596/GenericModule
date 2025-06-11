using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaAudioModule.Implement
{
	[CreateAssetMenu(fileName = "AudioModuleConfig", menuName = "Ciza/AudioModule/AudioModuleConfig")]
	public class AudioModuleConfig : ScriptableObject, IAudioModuleConfig
	{
		[SerializeField]
		protected string _poolRootName = "[AudioModule]";

		[Space]
		[SerializeField]
		protected string _poolPrefix = "";

		[SerializeField]
		protected string _poolSuffix = "s";

		[Space]
		[SerializeField]
		protected string _audioMixerGroupPath = "Master";

		[SerializeField]
		protected string _audioMixerParameter = "Master";

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
		protected AudioInfoMapList _infoMapList;

		public virtual string PoolRootName => _poolRootName;

		public virtual string PoolPrefix => _poolPrefix;
		public virtual string PoolSuffix => _poolSuffix;

		public virtual string AudioMixerGroupPath => _audioMixerGroupPath;
		public virtual string AudioMixerParameter => _audioMixerParameter;
		public virtual float DefaultVolume => _defaultVolume;

		public virtual bool TryGetRestrictContinuousPlay(out IRestrictContinuousPlay restrictContinuousPlay) =>
			_hasRestrictContinuousPlay.TryGetValue(out restrictContinuousPlay);

		public virtual string DefaultPrefabAddress => _defaultPrefabAddress;

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
	}
}