using System;
using UnityEngine.Scripting;

namespace CizaAudioModule
{
	[Serializable]
	public class AddressMapList : BMapList<AddressMapList.Map, string>
	{
		[Preserve]
		public AddressMapList() { }

		protected override Map CreateMap(string key, string value) =>
			new Map(key, value);

		[Serializable]
		public class Map : BMap<string>
		{
			[Preserve]
			public Map() : base("Default", string.Empty) { }

			[Preserve]
			public Map(string dataId, string address) : base(dataId, address) { }

			[Preserve]
			public Map(string dataId, bool isEnable, string address) : base(dataId, isEnable, address) { }
		}
	}
}