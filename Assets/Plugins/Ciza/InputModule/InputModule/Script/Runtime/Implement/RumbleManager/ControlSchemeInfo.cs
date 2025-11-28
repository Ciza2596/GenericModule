using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaInputModule
{
	[Serializable]
	public class ControlSchemeInfo : IControlSchemeInfo
	{
		[Space]
		[SerializeField]
		private float _lowFrequency;

		[SerializeField]
		private float _highFrequency;

		public float LowFrequency => _lowFrequency;
		public float HighFrequency => _highFrequency;

		[Preserve]
		public ControlSchemeInfo() { }

		[Preserve]
		public ControlSchemeInfo(float lowFrequency, float highFrequency)
		{
			_lowFrequency = lowFrequency;
			_highFrequency = highFrequency;
		}
	}
}