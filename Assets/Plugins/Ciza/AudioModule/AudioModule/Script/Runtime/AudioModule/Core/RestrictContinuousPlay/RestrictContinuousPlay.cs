using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAudioModule
{
	[Serializable]
	public class RestrictContinuousPlay : IRestrictContinuousPlay
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected float _duration;

		[SerializeField]
		protected int _maxConsecutiveCount;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual float Duration => _duration;
		public virtual int MaxConsecutiveCount => _maxConsecutiveCount;


		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public RestrictContinuousPlay() : this(0.1f, 1) { }

		[Preserve]
		public RestrictContinuousPlay(float duration, int maxConsecutiveCount)
		{
			_duration = duration;
			_maxConsecutiveCount = maxConsecutiveCount;
		}
	}
}