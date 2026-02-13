using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaTextModule
{
	[Serializable]
	public class TextAssetEnabler : BEnabler<TextAsset>
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected TextAsset _value;

		protected override TextAsset ValueImp
		{
			get => _value;
			set => _value = value;
		}

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public TextAssetEnabler() { }

		[Preserve]
		public TextAssetEnabler(bool isEnable) : base(isEnable) { }

		[Preserve]
		public TextAssetEnabler(TextAsset value) : base(value) { }

		[Preserve]
		public TextAssetEnabler(bool isEnable, TextAsset value) : base(isEnable, value) { }
	}
}