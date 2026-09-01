using System;
using UnityEngine.Scripting;

namespace CizaAudioModule
{
	[Serializable]
	public class AudioChannelInfoMapList : MapList<AudioChannelInfo>
	{
		[Preserve]
		public AudioChannelInfoMapList() { }
	}
}