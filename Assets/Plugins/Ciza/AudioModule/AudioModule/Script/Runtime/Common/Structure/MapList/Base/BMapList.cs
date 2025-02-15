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
        [SerializeReference]
        protected List<TMap> _maps = new List<TMap>();

        public Pair<TValue>[] Pairs
        {
            get
            {
                var pairs = new List<Pair<TValue>>();
                foreach (var map in _maps)
                    if (map.IsEnable)
                        pairs.Add(new Pair<TValue>(map.Key, map.Value));

                return pairs.ToArray();
            }
        }

        public string[] Keys
        {
            get
            {
                if (_maps is null)
                    return Array.Empty<string>();

                var allDataIds = new HashSet<string>();
                foreach (var map in _maps)
                    if (map.IsEnable)
                        allDataIds.Add(map.Key);
                return allDataIds.ToArray();
            }
        }

        public TValue[] Values
        {
            get
            {
                if (_maps is null)
                    return Array.Empty<TValue>();

                var allValues = new HashSet<TValue>();
                foreach (var map in _maps)
                    if (map.IsEnable)
                        allValues.Add(map.Value);
                return allValues.ToArray();
            }
        }

        public bool TryGetValue(string key, out TValue value)
        {
            if (_maps is null)
            {
                value = default;
                return false;
            }

            var map = _maps.FirstOrDefault(map => map.Key == key);
            if (map is null || !map.IsEnable)
            {
                value = default;
                return false;
            }

            value = map.Value;
            return true;
        }

        [Preserve]
        protected BMapList() { }


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

        protected abstract TMap CreateMap(string key, TValue value);
    }
}