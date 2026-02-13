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
		protected string _csv;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual bool TryGetCsv(out string csv)
		{
			if (!_csv.CheckHasValue())
			{
				csv = string.Empty;
				return false;
			}

			csv = _csv;
			return true;
		}

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public TextModuleConfigInput() : this(string.Empty) { }

		[Preserve]
		public TextModuleConfigInput(string csv) =>
			_csv = csv;
	}
}