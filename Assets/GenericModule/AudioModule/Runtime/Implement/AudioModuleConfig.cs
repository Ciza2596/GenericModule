using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaAudioModule.Implement
{
	[CreateAssetMenu(fileName = "AudioModuleConfig", menuName = "Ciza/AudioModule/AudioModuleConfig")]
	public class AudioModuleConfig : ScriptableObject, IAudioModuleConfig
	{
		[SerializeField]
		private string _audioMixerGroupPath = "Master";

		[Space]
		[SerializeField]
		private string _poolRootName = "[AudioModule]";

		[SerializeField]
		private string _poolPrefix = "";

		[SerializeField]
		private string _poolSuffix = "s";

		[SerializeField]
		private string _defaultPrefabAddress;

		[Space]
		[SerializeField]
		private AudioInfo[] _audioInfos;

		public string AudioMixerGroupPath => _audioMixerGroupPath;

		public string PoolRootName => _poolRootName;
		public string PoolPrefix   => _poolPrefix;
		public string PoolSuffix   => _poolSuffix;

		public string DefaultPrefabAddress => _defaultPrefabAddress;

		public IReadOnlyDictionary<string, IAudioInfo> CreateAudioInfoMapDataId()
		{
			Assert.IsNotNull(_audioInfos, "[AudioModuleConfig::GetAudioInfoMap] AudioInfos is null.");

			var audioInfoMap = new Dictionary<string, IAudioInfo>();

			if (_audioInfos is null)
				return audioInfoMap;

			foreach (var audioInfo in _audioInfos)
				audioInfoMap.Add(audioInfo.DataId, audioInfo);

			return audioInfoMap;
		}

		[Serializable]
		private class AudioInfo : IAudioInfo
		{
			[SerializeField]
			private string _dataId;

			[Space]
			[SerializeField]
			private string _clipDataId;

			[SerializeField]
			private string _prefabDataId;

			public string DataId => _dataId.ToLower();

			public string ClipAddress   => _clipDataId;
			public string PrefabAddress => _prefabDataId;
		}
	}
}
