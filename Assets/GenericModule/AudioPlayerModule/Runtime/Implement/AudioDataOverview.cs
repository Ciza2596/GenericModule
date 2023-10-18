using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaAudioPlayerModule.Implement
{
	[CreateAssetMenu(fileName = "AudioDataOverview", menuName = "Ciza/AudioPlayerModule/AudioDataOverview", order = -100)]
	public class AudioDataOverview : ScriptableObject
	{
		[SerializeField]
		private AudioInfo[] _audioDatas;

		public Dictionary<string, IAudioInfo> GetAudioDataMap()
		{
			Assert.IsNotNull(_audioDatas, "[AudioDataOverview::GetAudioDataMap] AudioDatas is null.");

			var audioDataMap = new Dictionary<string, IAudioInfo>();

			foreach (var audioData in _audioDatas)
				audioDataMap.Add(audioData.ClipAddress, audioData);

			return audioDataMap;
		}

		[Serializable]
		private class AudioInfo : IAudioInfo
		{
			[SerializeField]
			private string _clipDataId;

			[SerializeField]
			private string _prefabDataId;

			[Range(0, 1)]
			[SerializeField]
			private float _spatialBlend;

			public string DataId        { get; }
			public string ClipAddress   => _clipDataId;
			public string PrefabAddress => _prefabDataId;
			public float  SpatialBlend  => _spatialBlend;
		}
	}
}
