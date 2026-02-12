using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaInputModule
{
	[Serializable]
	public class VirtualMouseInfoMapList : BMapList<VirtualMouseInfoMapList.Map, VirtualMouseInfo>
	{
		[Preserve]
		public VirtualMouseInfoMapList() { }

		protected override Map CreateMap(string key, VirtualMouseInfo value) =>
			new Map(value);

		[Serializable]
		public class Map : BMap<VirtualMouseInfo>
		{
			[SerializeField]
			protected VirtualMouseInfo _value;

			public override string Key => _value.PlayerIndex.ToString();

			public override VirtualMouseInfo Value => _value;

			[Preserve]
			public Map() : this(new VirtualMouseInfo()) { }

			[Preserve]
			public Map(VirtualMouseInfo value) : base() => 
				_value = value;
			
			[Preserve]
			public Map(bool isEnable, VirtualMouseInfo value) : base(isEnable) =>
				_value = value;
		}
	}
}