using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaTextModule
{
	[Serializable]
	public abstract class BEnabler<TValueImp> : BEnabler<TValueImp, TValueImp>
	{
		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		protected BEnabler() { }

		[Preserve]
		protected BEnabler(bool isEnable) : base(isEnable) { }

		[Preserve]
		protected BEnabler(TValueImp value) : base(value) { }

		[Preserve]
		protected BEnabler(bool isEnable, TValueImp value) : base(isEnable, value) { }
	}

	[Serializable]
	public abstract class BEnabler<TValueImp, TValue> : IEnabler<TValue> where TValueImp : TValue
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected bool _isEnable;

		protected abstract TValueImp ValueImp { get; set; }

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual bool IsEnable => _isEnable;
		public virtual TValue Value => ValueImp;

		public virtual bool TryGetValue(out TValue value)
		{
			value = Value;
			return _isEnable && value != null;
		}

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		protected BEnabler() { }

		[Preserve]
		protected BEnabler(bool isEnable) =>
			_isEnable = isEnable;

		[Preserve]
		protected BEnabler(TValueImp value) : this(true, value) { }


		[Preserve]
		protected BEnabler(bool isEnable, TValueImp value)
		{
			_isEnable = isEnable;
			SetValue(value);
		}

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual void SetIsEnable(bool isEnable) =>
			_isEnable = isEnable;

		public virtual void SetValue(TValueImp value) =>
			ValueImp = value;
	}
}