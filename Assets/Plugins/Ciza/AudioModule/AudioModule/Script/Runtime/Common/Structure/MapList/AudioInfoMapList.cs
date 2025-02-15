using System;
using UnityEngine.Scripting;

namespace CizaAudioModule
{
	[Serializable]
	public class AudioInfoMapList : BMapList<AudioInfoMapList.Map, AudioInfo>
	{
		[Preserve]
		public AudioInfoMapList() { }

		protected override Map CreateMap(string key, AudioInfo value) =>
			new Map(key, value);

		[Serializable]
		public class Map : BMap<AudioInfo>
		{
			[Preserve]
			public Map() : base("Default", null) { }

			[Preserve]
			public Map(string dataId, AudioInfo audioInfo) : base(dataId, audioInfo) { }

			[Preserve]
			public Map(string dataId, bool isEnable, AudioInfo audioInfo) : base(dataId, isEnable, audioInfo) { }
		}
	}
}