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

		public virtual bool TryGetCsv(out string csv)
		{
			if (!_hasCsvTextAsset.TryGetValue(out var csvTextAsset) || !csvTextAsset.text.CheckHasValue())
			{
				csv = string.Empty;
				return false;
			}

			csv = csvTextAsset.text;
			return true;
		}

		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_hasCsvTextAsset = null;
		}
	}
}