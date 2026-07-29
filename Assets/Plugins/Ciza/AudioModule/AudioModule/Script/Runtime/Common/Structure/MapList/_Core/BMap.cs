using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAudioModule
{
	[Serializable]
	public abstract class BMap
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		[HideInInspector]
		protected bool _isEnable;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public abstract string Key { get; }
		public virtual bool IsEnable => _isEnable;

		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		protected BMap(bool isEnable) =>
			_isEnable = isEnable;
	}

	[Serializable]
	public abstract class BMap<TValue> : BMap
	{
		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public abstract TValue Value { get; }

		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		protected BMap() : this(true) { }

		[Preserve]
		protected BMap(bool isEnable) : base(isEnable) { }
	}
}