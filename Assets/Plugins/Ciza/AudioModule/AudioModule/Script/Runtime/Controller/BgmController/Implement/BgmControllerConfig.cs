using System;
using System.Linq;
using UnityEngine;

namespace CizaAudioModule.Implement
{
	[CreateAssetMenu(fileName = "BgmControllerConfig", menuName = "Ciza/AudioModule/BgmControllerConfig", order = 200)]
	public class BgmControllerConfig : ScriptableObject, IBgmControllerConfig
	{
		[SerializeField]
		protected float _fadeTime = 0.5f;

		[Space]
		[SerializeField]
		[OverrideDrawer]
		protected string[] _bgmDataIds;

		public virtual float FadeTime => _fadeTime;

		public virtual string[] BgmDataIds => _bgmDataIds != null ? _bgmDataIds.ToHashSet().ToArray() : Array.Empty<string>();
	}
}