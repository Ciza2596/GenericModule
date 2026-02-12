using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaInputModule
{
	[Serializable]
	public class ScreenPaddingEnabler : BEnabler<ScreenPadding>
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected ScreenPadding _value = new ScreenPadding();

		protected override ScreenPadding ValueImp
		{
			get => _value;
			set => _value = value;
		}

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public ScreenPaddingEnabler() { }

		[Preserve]
		public ScreenPaddingEnabler(bool isEnable) : base(isEnable) { }

		[Preserve]
		public ScreenPaddingEnabler(ScreenPadding value) : base(value) { }

		[Preserve]
		public ScreenPaddingEnabler(bool isEnable, ScreenPadding value) : base(isEnable, value) { }
	}

	[Serializable]
	public class ScreenPadding
	{
		[SerializeField]
		private bool _isByRatio;

		[SerializeField]
		private RectOffset _padding;

		public bool IsByRatio => _isByRatio;
		public RectOffset Padding => _padding;


		[Preserve]
		public ScreenPadding() : this(false, new RectOffset()) { }

		[Preserve]
		public ScreenPadding(bool isByRatio, RectOffset padding)
		{
			_isByRatio = isByRatio;
			_padding = padding;
		}
	}
}