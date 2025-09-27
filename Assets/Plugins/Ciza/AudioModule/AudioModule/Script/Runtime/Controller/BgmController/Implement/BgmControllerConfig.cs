using System;
using System.Linq;
using UnityEngine;

namespace CizaAudioModule.Implement
{
	[CreateAssetMenu(fileName = "BgmControllerConfig", menuName = "Ciza/AudioModule/BgmControllerConfig", order = 200)]
	public class BgmControllerConfig : ScriptableObject, IBgmControllerConfig
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected float _fadeTime;

		[Space]
		[SerializeField]
		[OverrideDrawer]
		protected string[] _bgmDataIds;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual float FadeTime => _fadeTime;

		public virtual string[] BgmDataIds => _bgmDataIds != null ? _bgmDataIds.ToHashSet().ToArray() : Array.Empty<string>();

		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_fadeTime = 0.5f;
			_bgmDataIds = Array.Empty<string>();
		}
	}
}