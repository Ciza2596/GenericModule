using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAudioModule
{
	[Serializable]
	public class MultipleAudioChannelInfoEnabler : BEnabler<MultipleAudioChannelInfo, IMultipleAudioChannelInfo>
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected MultipleAudioChannelInfo _value;

		protected override MultipleAudioChannelInfo ValueImp
		{
			get => _value;
			set => _value = value;
		}

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public MultipleAudioChannelInfoEnabler() { }

		[Preserve]
		public MultipleAudioChannelInfoEnabler(bool isEnable) : base(isEnable) { }

		[Preserve]
		public MultipleAudioChannelInfoEnabler(MultipleAudioChannelInfo value) : base(value) { }

		[Preserve]
		public MultipleAudioChannelInfoEnabler(bool isEnable, MultipleAudioChannelInfo value) : base(isEnable, value) { }
	}
}