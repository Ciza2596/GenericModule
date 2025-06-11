using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAudioModule
{
	[Serializable]
	public class AudioInfoMapList : BMapList<AudioInfoMapList.Map, AudioInfo>
	{
		[Preserve]
		public AudioInfoMapList() { }

		protected override Map CreateMap(string key, AudioInfo value) =>
			new Map(value);

		[Serializable]
		public class Map : BMap<AudioInfo>
		{
			[SerializeField]
			protected AudioInfo _value;

			public override string Key => Value.DataId;

			public override AudioInfo Value => _value;

			[Preserve]
			public Map() : this(new AudioInfo()) { }

			[Preserve]
			public Map(AudioInfo value) : base() =>
				_value = value;

			[Preserve]
			public Map(bool isEnable, AudioInfo value) : base(isEnable) =>
				_value = value;
		}
	}
}