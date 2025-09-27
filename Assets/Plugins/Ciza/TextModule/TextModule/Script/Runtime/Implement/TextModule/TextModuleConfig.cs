using UnityEngine;

namespace CizaTextModule.Implement
{
	[CreateAssetMenu(fileName = "TextModuleConfig", menuName = "Ciza/TextModule/TextModuleConfig")]
	public class TextModuleConfig : ScriptableObject, ITextModuleConfig
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected bool _isCustomDefaultCategory;

		[SerializeField]
		protected string _customCustomDefaultCategory;

		[Space]
		[SerializeField]
		protected TextAsset _csvTextAsset;

		[Space]
		[SerializeField]
		protected bool _isShowWarningLog;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual bool IsCustomDefaultCategory => _isCustomDefaultCategory;
		public virtual string CustomDefaultCategory => _customCustomDefaultCategory;

		public virtual string CsvText => _csvTextAsset.text;

		public virtual bool IsShowWarningLog => _isShowWarningLog;

		public virtual void Reset()
		{
			_isCustomDefaultCategory = false;
			_customCustomDefaultCategory = string.Empty;

			_csvTextAsset = null;
			_isShowWarningLog = true;
		}
	}
}