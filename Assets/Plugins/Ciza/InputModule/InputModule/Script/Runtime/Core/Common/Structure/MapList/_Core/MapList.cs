using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaInputModule
{
	[Serializable]
	public class MapList<TValue> : BMapList<MapList<TValue>.Map, TValue>
	{
		[Preserve]
		public MapList() { }

		protected override Map CreateMap(string key, TValue value) =>
			new Map(key, value);

		[Serializable]
		public class Map : BMap<TValue>
		{
			[SerializeField]
			protected string _key;

			[SerializeField]
			protected TValue _value;

			public override string Key => _key;

			public override TValue Value => _value;

			[Preserve]
			public Map() : this("Default", default) { }

			[Preserve]
			public Map(string key, TValue value) : base()
			{
				_key = key;
				_value = value;
			}

			[Preserve]
			public Map(string key, bool isEnable, TValue value) : base(isEnable)
			{
				_key = key;
				_value = value;
			}
		}
	}
}