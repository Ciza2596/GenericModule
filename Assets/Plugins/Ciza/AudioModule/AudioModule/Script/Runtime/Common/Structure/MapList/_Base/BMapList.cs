using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAudioModule
{
	[Serializable]
	public abstract class BMapList<TMap, TValue> where TMap : BMap<TValue>
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected List<TMap> _maps = new List<TMap>();

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual KeyValuePair<string, TValue>[] KeyValuePairs =>
			_maps.Where(map => map.IsEnable).Select(map => new KeyValuePair<string, TValue>(map.Key, map.Value)).ToArray();

		public virtual string[] Keys =>
			_maps.Where(map => map.IsEnable).Select(map => map.Key).Distinct().ToArray();

		public virtual TValue[] Values =>
			_maps.Where(map => map.IsEnable).Select(map => map.Value).Distinct().ToArray();

		public virtual bool TryGetValue(string key, out TValue value)
		{
			var map = _maps.FirstOrDefault(map => map.IsEnable && map.Key == key);
			if (map is null)
			{
				value = default;
				return false;
			}

			value = map.Value;
			return true;
		}

		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		protected BMapList() { }


		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual Dictionary<string, TValue> ToDictionary()
		{
			var dictionary = new Dictionary<string, TValue>();
			foreach (var keyValuePair in KeyValuePairs)
				dictionary.Add(keyValuePair.Key, keyValuePair.Value);
			return dictionary;
		}

		public virtual Dictionary<string, TValueType> ToDictionary<TValueType>() where TValueType : class
		{
			var dictionary = new Dictionary<string, TValueType>();
			foreach (var keyValuePair in KeyValuePairs)
				dictionary.Add(keyValuePair.Key, keyValuePair.Value as TValueType);
			return dictionary;
		}

		public virtual void Add(string key, TValue value)
		{
			if (Keys.Contains(key))
				return;
			_maps.Add(CreateMap(key, value));
		}

		public virtual void Remove(string key)
		{
			if (!Keys.Contains(key))
				return;

			foreach (var map in _maps.ToArray())
				if (map.Key == key)
					_maps.Remove(map);
		}

		public virtual void Clear()
		{
			foreach (var key in Keys)
				Remove(key);
		}

		public virtual void Reset(KeyValuePair<string, TValue>[] keyValuePairs)
		{
			Clear();
			foreach (var keyValuePair in keyValuePairs)
				Add(keyValuePair.Key, keyValuePair.Value);
		}

		// PROTECT METHOD: --------------------------------------------------------------------

		protected abstract TMap CreateMap(string key, TValue value);
	}
}