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
}