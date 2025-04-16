using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAudioModule
{
    [Serializable]
    public abstract class BMap
    {
        [SerializeField]
        protected string _key;

        [SerializeField]
        [HideInInspector]
        protected bool _isEnable;

        public virtual string Key => _key;
        public virtual bool IsEnable => _isEnable;

        [Preserve]
        protected BMap(string key, bool isEnable)
        {
            _key = key;
            _isEnable = isEnable;
        }
    }

    [Serializable]
    public class BMap<TValue> : BMap
    {
        [SerializeField]
        protected TValue _value;

        public virtual TValue Value => _value;

        [Preserve]
        public BMap(string key, TValue value) : this(key, true, value) { }

        [Preserve]
        public BMap(string key, bool isEnable, TValue value) : base(key, isEnable)
        {
            _value = value;
        }
    }
}