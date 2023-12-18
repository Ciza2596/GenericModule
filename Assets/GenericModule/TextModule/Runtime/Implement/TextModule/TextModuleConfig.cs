using UnityEngine;

namespace CizaTextModule.Implement
{
	[CreateAssetMenu(fileName = "TextModuleConfig", menuName = "Ciza/TextModule/TextModuleConfig")]
	public class TextModuleConfig : ScriptableObject, ITextModuleConfig
	{
		[SerializeField]
		private bool _isCustomDefaultCategory = false;

		[SerializeField]
		private string _customCustomDefaultCategory;

		[Space]
		[SerializeField]
		private TextAsset _csvTextAsset;

		[Space]
		[SerializeField]
		private bool _isShowWarningLog = true;

		public bool   IsCustomDefaultCategory => _isCustomDefaultCategory;
		public string CustomDefaultCategory   => _customCustomDefaultCategory;

		public string CsvText => _csvTextAsset.text;

		public bool IsShowWarningLog => _isShowWarningLog;
	}
}
