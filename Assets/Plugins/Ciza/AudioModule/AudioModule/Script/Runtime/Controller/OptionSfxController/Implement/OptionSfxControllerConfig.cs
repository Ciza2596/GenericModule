using UnityEngine;

namespace CizaAudioModule.Implement
{
	[CreateAssetMenu(fileName = "OptionSfxControllerConfig", menuName = "Ciza/AudioModule/OptionSfxControllerConfig", order = 201)]
	public class OptionSfxControllerConfig : ScriptableObject, IOptionSfxControllerConfig
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected string _selectSfxDataId = "UI_Select";

		[Space]
		[SerializeField]
		protected string _confirmSfxDataId = "UI_Confirm";

		[SerializeField]
		protected string _cantConfirmSfxDataId = "UI_CantConfirm";

		[Space]
		[SerializeField]
		protected string _cancelSfxDataId = "UI_Cancel";

		[Space]
		[SerializeField]
		protected string _settingsShowSfxDataId = "UI_SettingsShow";

		[SerializeField]
		protected string _settingsHideSfxDataId = "UI_SettingsHide";

		[Space]
		[SerializeField]
		protected string _dialogContinueSfxDataId = "UI_DialogContinue";

		[SerializeField]
		protected string _dialogFunctionSfxDataId = "UI_DialogFunction";


		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual bool TryGetSelectSfxDataId(out string selectSfxDataId) =>
			TryGetSfxDataId(_selectSfxDataId, out selectSfxDataId);


		public virtual bool TryGetConfirmSfxDataId(out string confirmSfxDataId) =>
			TryGetSfxDataId(_confirmSfxDataId, out confirmSfxDataId);

		public virtual bool TryGetCantConfirmSfxDataId(out string cantConfirmSfxDataId) =>
			TryGetSfxDataId(_cantConfirmSfxDataId, out cantConfirmSfxDataId);


		public virtual bool TryGetCancelSfxDataId(out string cancelSfxDataId) =>
			TryGetSfxDataId(_cancelSfxDataId, out cancelSfxDataId);


		public virtual bool TryGetSettingsShowSfxDataId(out string settingsShowSfxDataId) =>
			TryGetSfxDataId(_settingsShowSfxDataId, out settingsShowSfxDataId);

		public virtual bool TryGetSettingsHideSfxDataId(out string settingsHideSfxDataId) =>
			TryGetSfxDataId(_settingsHideSfxDataId, out settingsHideSfxDataId);


		public virtual bool TryGetDialogContinueSfxDataId(out string dialogContinueSfxDataId) =>
			TryGetSfxDataId(_dialogContinueSfxDataId, out dialogContinueSfxDataId);

		public virtual bool TryGetDialogFunctionSfxDataId(out string dialogFunctionSfxDataId) =>
			TryGetSfxDataId(_dialogFunctionSfxDataId, out dialogFunctionSfxDataId);


		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_selectSfxDataId = "UI_Select";

			_confirmSfxDataId = "UI_Confirm";
			_cantConfirmSfxDataId = "UI_CantConfirm";

			_cancelSfxDataId = "UI_Cancel";

			_settingsShowSfxDataId = "UI_SettingsShow";
			_settingsHideSfxDataId = "UI_SettingsHide";

			_dialogContinueSfxDataId = "UI_DialogContinue";
			_dialogFunctionSfxDataId = "UI_DialogFunction";
		}


		// PROTECT METHOD: --------------------------------------------------------------------

		protected bool TryGetSfxDataId(string value, out string sfxDataId)
		{
			if (!value.CheckHasValue())
			{
				sfxDataId = string.Empty;
				return false;
			}

			sfxDataId = value;
			return true;
		}
	}
}