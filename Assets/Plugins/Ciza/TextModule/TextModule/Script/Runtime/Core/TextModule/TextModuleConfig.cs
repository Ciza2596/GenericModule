using UnityEngine;

namespace CizaTextModule
{
	[CreateAssetMenu(fileName = "TextModuleConfig", menuName = "Ciza/TextModule/TextModuleConfig")]
	public class TextModuleConfig : ScriptableObject /*, ITextModuleConfig*/
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected TextAsset _csvTextAsset;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string TryGetCsvText => _csvTextAsset.text;

		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_csvTextAsset = null;
		}
	}
}