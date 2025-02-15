using System;
using UnityEngine;

namespace CizaAudioModule
{
	[Serializable]
	public class AudioInfo : IAudioInfo
	{
		[SerializeField]
		private string _dataId;

		[Space]
		[SerializeField]
		private string _clipAddress;

		[SerializeField]
		private string _prefabAddress;

		public virtual string DataId => _dataId;

		public virtual string ClipAddress => _clipAddress;
		public virtual string PrefabAddress => _prefabAddress;
	}
}