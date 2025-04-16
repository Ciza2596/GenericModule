using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAudioModule
{
	[Serializable]
	public class RestrictContinuousPlayEnabler : BEnabler<RestrictContinuousPlay, IRestrictContinuousPlay>
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected RestrictContinuousPlay _value;

		protected override RestrictContinuousPlay ValueImp
		{
			get => _value;
			set => _value = value;
		}

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public RestrictContinuousPlayEnabler() { }

		[Preserve]
		public RestrictContinuousPlayEnabler(bool isEnable) : base(isEnable) { }

		[Preserve]
		public RestrictContinuousPlayEnabler(RestrictContinuousPlay value) : base(value) { }

		[Preserve]
		public RestrictContinuousPlayEnabler(bool isEnable, RestrictContinuousPlay value) : base(isEnable, value) { }
	}

	[Serializable]
	public class RestrictContinuousPlay : IRestrictContinuousPlay
	{
		[SerializeField]
		protected float _duration = 0.1f;

		[SerializeField]
		protected int _maxConsecutiveCount = 1;

		public virtual float Duration => _duration;
		public virtual int MaxConsecutiveCount => _maxConsecutiveCount;
	}
}