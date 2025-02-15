using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAudioModule
{
	[Serializable]
	public abstract class BEnabler<TValueImp> : BEnabler<TValueImp, TValueImp> { }

	[Serializable]
	public abstract class BEnabler<TValueImp, TValue> where TValueImp : TValue
	{
		[SerializeField]
		protected bool _isEnable;

		[SerializeField]
		protected TValueImp _value;

		public bool IsEnable => _isEnable;
		public TValue Value => _value;

		public virtual bool TryGetValue(out TValue value)
		{
			value = _value;
			return _isEnable && value != null;
		}

		[Preserve]
		protected BEnabler() { }

		public virtual void SetIsEnable(bool isEnable) =>
			_isEnable = isEnable;

		public virtual void SetValue(TValueImp value) =>
			_value = value;
	}
}