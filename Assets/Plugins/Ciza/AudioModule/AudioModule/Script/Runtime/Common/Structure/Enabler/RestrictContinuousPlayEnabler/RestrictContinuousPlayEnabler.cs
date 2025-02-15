using System;
using UnityEngine;

namespace CizaAudioModule
{
	[Serializable]
	public class RestrictContinuousPlayEnabler : BEnabler<RestrictContinuousPlay, IRestrictContinuousPlay> { }

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