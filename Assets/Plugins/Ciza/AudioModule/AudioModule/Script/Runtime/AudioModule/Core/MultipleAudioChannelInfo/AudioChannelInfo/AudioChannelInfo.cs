using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAudioModule
{
	[Serializable]
	public class AudioChannelInfo : IAudioChannelInfo, IZomeraphyPanel
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected string _audioMixerGroupPath;

		[SerializeField]
		protected string _audioMixerVolumeParameter;

		[Range(0, 1)]
		[SerializeField]
		protected float _defaultVolume;


		// PUBLIC VARIABLE: ---------------------------------------------------------------------


		public virtual string AudioMixerGroupPath => _audioMixerGroupPath;
		public virtual string AudioMixerVolumeParameter => _audioMixerVolumeParameter;
		public virtual float DefaultVolume => _defaultVolume;

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public AudioChannelInfo() : this(string.Empty, string.Empty, 1f) { }

		[Preserve]
		public AudioChannelInfo(string audioMixerGroupPath, string audioMixerVolumeParameter, float defaultVolume)
		{
			_audioMixerGroupPath = audioMixerGroupPath;
			_audioMixerVolumeParameter = audioMixerVolumeParameter;
			_defaultVolume = defaultVolume;
		}
	}
}