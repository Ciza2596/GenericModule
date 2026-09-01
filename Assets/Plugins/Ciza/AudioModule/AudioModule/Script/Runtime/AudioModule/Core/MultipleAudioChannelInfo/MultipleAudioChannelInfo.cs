using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAudioModule
{
	[Serializable]
	public class MultipleAudioChannelInfo : IMultipleAudioChannelInfo, IZomeraphyPanel
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected AudioChannelInfo _extraChannelInfo;

		[Space]
		[Space]
		[SerializeField]
		protected AudioChannelInfoMapList _channelInfoMapList;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual IAudioChannelInfo ExtraChannelInfo => _extraChannelInfo;

		public virtual bool TryGetChannelInfo(string dataId, out IAudioChannelInfo channelInfo)
		{
			if (!_channelInfoMapList.TryGetValue(dataId, out var channelInfoImp))
			{
				channelInfo = null;
				return false;
			}

			channelInfo = channelInfoImp;
			return true;
		}


		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public MultipleAudioChannelInfo() : this(new AudioChannelInfo(), new AudioChannelInfoMapList()) { }

		[Preserve]
		public MultipleAudioChannelInfo(AudioChannelInfo extraChannelInfo, AudioChannelInfoMapList channelInfoMapList)
		{
			_extraChannelInfo = extraChannelInfo;
			_channelInfoMapList = channelInfoMapList;
		}
	}
}