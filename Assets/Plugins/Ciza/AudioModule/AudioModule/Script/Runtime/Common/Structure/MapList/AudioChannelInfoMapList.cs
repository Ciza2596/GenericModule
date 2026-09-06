using System;
using UnityEngine.Scripting;

namespace CizaAudioModule
{
	[Serializable]
	public class AudioChannelInfoMapList : MapList<AudioChannelInfo>
	{
		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public AudioChannelInfoMapList() { }

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public new AudioChannelInfoMapList Copy() =>
			Copy<AudioChannelInfoMapList>();
	}
}