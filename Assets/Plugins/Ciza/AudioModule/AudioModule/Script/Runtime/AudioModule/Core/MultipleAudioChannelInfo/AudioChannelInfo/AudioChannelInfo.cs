using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAudioModule
{
	[Serializable]
	public class AudioChannelInfo : IAudioChannelInfo, IZomeraphyPanel
	{
		// CONST & STATIC: -----------------------------------------------------------------------

		public const float MIN_MIXER_VOLUME = 0f;
		public const float MAX_MIXER_VOLUME = 1f;

		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected string _mixerGroupPath;

		[SerializeField]
		protected string _mixerVolumeParameter;

		[Range(0, 1)]
		[SerializeField]
		protected float _defaultMixerVolume;


		// PUBLIC VARIABLE: ---------------------------------------------------------------------


		public virtual string MixerGroupPath => _mixerGroupPath;
		public virtual string MixerVolumeParameter => _mixerVolumeParameter;

		public virtual float MinMixerVolume => MIN_MIXER_VOLUME;
		public virtual float MaxMixerVolume => MAX_MIXER_VOLUME;
		public virtual float DefaultMixerVolume => _defaultMixerVolume;

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public AudioChannelInfo() : this(string.Empty, string.Empty, 1f) { }

		[Preserve]
		public AudioChannelInfo(string mixerGroupPath, string mixerVolumeParameter, float defaultMixerVolume)
		{
			_mixerGroupPath = mixerGroupPath;
			_mixerVolumeParameter = mixerVolumeParameter;
			_defaultMixerVolume = defaultMixerVolume;
		}
	}
}