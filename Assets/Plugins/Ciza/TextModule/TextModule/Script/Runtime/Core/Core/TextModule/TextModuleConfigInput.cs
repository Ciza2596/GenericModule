using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaTextModule
{
	[Serializable]
	public class TextModuleConfigInput : ITextModuleConfig
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected string _csvText;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual bool TryGetCsvText(out string csvText)
		{
			if (!_csvText.CheckHasValue())
			{
				csvText = string.Empty;
				return false;
			}

			csvText = _csvText;
			return true;
		}

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public TextModuleConfigInput() : this(string.Empty) { }

		[Preserve]
		public TextModuleConfigInput(string csvText) =>
			_csvText = csvText;
	}
}