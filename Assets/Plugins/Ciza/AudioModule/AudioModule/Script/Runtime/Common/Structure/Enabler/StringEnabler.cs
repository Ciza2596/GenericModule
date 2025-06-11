using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAudioModule
{
	[Serializable]
	public class StringEnabler : BEnabler<string>
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected string _value;

		protected override string ValueImp
		{
			get => _value;
			set => _value = value;
		}

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public StringEnabler() { }

		[Preserve]
		public StringEnabler(bool isEnable) : base(isEnable) { }

		[Preserve]
		public StringEnabler(string value) : base(value) { }

		[Preserve]
		public StringEnabler(bool isEnable, string value) : base(isEnable, value) { }
	}
}