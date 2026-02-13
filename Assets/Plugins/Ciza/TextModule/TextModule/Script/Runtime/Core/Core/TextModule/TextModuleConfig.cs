using UnityEngine;

namespace CizaTextModule
{
	[CreateAssetMenu(fileName = "TextModuleConfig", menuName = "Ciza/TextModule/TextModuleConfig")]
	public class TextModuleConfig : ScriptableObject, ITextModuleConfig
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected TextAssetEnabler _hasCsvTextAsset;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual bool TryGetCsvText(out string cvsText)
		{
			if (!_hasCsvTextAsset.TryGetValue(out var csvTextAsset) || !csvTextAsset.text.CheckHasValue())
			{
				cvsText = string.Empty;
				return false;
			}

			cvsText = csvTextAsset.text;
			return true;
		}

		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_hasCsvTextAsset = null;
		}
	}
}