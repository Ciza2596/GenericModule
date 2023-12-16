using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaInputModule.Implement
{
	[Serializable]
	public class RumbleInfo : IRumbleInfo
	{
		[SerializeField]
		private string _dataId;

		[Space]
		[SerializeField]
		private float _duration;

		[Space]
		[SerializeField]
		private float _lowFrequency;

		[SerializeField]
		private float _highFrequency;

		[Preserve]
		public RumbleInfo() { }

		[Preserve]
		public RumbleInfo(string dataId, float duration, float lowFrequency, float highFrequency)
		{
			_dataId        = dataId;
			_duration      = duration;
			_lowFrequency  = lowFrequency;
			_highFrequency = highFrequency;
		}

		public string DataId => _dataId;

		public float Duration => _duration;

		public float LowFrequency  => _lowFrequency;
		public float HighFrequency => _highFrequency;
	}
}
