using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAudioModule.Implement
{
	[CreateAssetMenu(fileName = "MenuAudioControllerConfig", menuName = "Ciza/AudioModule/MenuAudioControllerConfig", order = 101)]
	public class MenuAudioControllerConfig : ScriptableObject, IMenuAudioControllerConfig
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected BgmSettings _bgmSettings;

		[Space]
		[SerializeField]
		private OptionSfxSettings _optionSfxSettings;


		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual float FadeTime => _bgmSettings.FadeTime;
		public virtual string[] BgmDataIds => _bgmSettings.BgmDataIds;


		public virtual bool TryGetSelectSfxDataId(out string selectSfxDataId) =>
			_optionSfxSettings.TryGetSelectSfxDataId(out selectSfxDataId);


		public virtual bool TryGetConfirmSfxDataId(out string confirmSfxDataId) =>
			_optionSfxSettings.TryGetConfirmSfxDataId(out confirmSfxDataId);

		public virtual bool TryGetCantConfirmSfxDataId(out string cantConfirmSfxDataId) =>
			_optionSfxSettings.TryGetCantConfirmSfxDataId(out cantConfirmSfxDataId);


		public virtual bool TryGetCancelSfxDataId(out string cancelSfxDataId) =>
			_optionSfxSettings.TryGetCancelSfxDataId(out cancelSfxDataId);


		public virtual bool TryGetSettingsShowSfxDataId(out string settingsShowSfxDataId) =>
			_optionSfxSettings.TryGetSettingsShowSfxDataId(out settingsShowSfxDataId);

		public virtual bool TryGetSettingsHideSfxDataId(out string settingsHideSfxDataId) =>
			_optionSfxSettings.TryGetSettingsHideSfxDataId(out settingsHideSfxDataId);


		public virtual bool TryGetDialogContinueSfxDataId(out string dialogContinueSfxDataId) =>
			_optionSfxSettings.TryGetDialogContinueSfxDataId(out dialogContinueSfxDataId);

		public virtual bool TryGetDialogFunctionSfxDataId(out string dialogFunctionSfxDataId) =>
			_optionSfxSettings.TryGetDialogFunctionSfxDataId(out dialogFunctionSfxDataId);


		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_bgmSettings = new BgmSettings();
			_optionSfxSettings = new OptionSfxSettings();
		}

		[Serializable]
		protected class BgmSettings : IZomeraphyPanel
		{
			// VARIABLE: -----------------------------------------------------------------------------

			[SerializeField]
			protected float _fadeTime;

			[Space]
			[SerializeField]
			[OverrideDrawer]
			protected string[] _bgmDataIds;

			// PUBLIC VARIABLE: ---------------------------------------------------------------------

			public virtual float FadeTime => _fadeTime;
			public virtual string[] BgmDataIds => _bgmDataIds != null ? _bgmDataIds.ToHashSet().ToArray() : Array.Empty<string>();


			// CONSTRUCTOR: ------------------------------------------------------------------------

			[Preserve]
			public BgmSettings() : this(0.5f, Array.Empty<string>()) { }

			[Preserve]
			public BgmSettings(float fadeTime, string[] bgmDataIds)
			{
				_fadeTime = fadeTime;
				_bgmDataIds = bgmDataIds;
			}
		}

		[Serializable]
		protected class OptionSfxSettings : IZomeraphyPanel
		{
			// VARIABLE: -----------------------------------------------------------------------------

			[SerializeField]
			protected string _selectSfxDataId;

			[Space]
			[SerializeField]
			protected string _confirmSfxDataId;

			[SerializeField]
			protected string _cantConfirmSfxDataId;

			[Space]
			[SerializeField]
			protected string _cancelSfxDataId;

			[Space]
			[SerializeField]
			protected string _settingsShowSfxDataId;

			[SerializeField]
			protected string _settingsHideSfxDataId;

			[Space]
			[SerializeField]
			protected string _dialogContinueSfxDataId;

			[SerializeField]
			protected string _dialogFunctionSfxDataId;


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
			[Preserve]
			public OptionSfxSettings() : this("UI_Select", "UI_Confirm", "UI_CantConfirm", "UI_Cancel", "UI_SettingsShow", "UI_SettingsHide", "UI_DialogContinue", "UI_DialogFunction") { }

			[Preserve]
			public OptionSfxSettings(string selectSfxDataId, string confirmSfxDataId, string cantConfirmSfxDataId, string cancelSfxDataId, string settingsShowSfxDataId, string settingsHideSfxDataId, string dialogContinueSfxDataId, string dialogFunctionSfxDataId)
			{
				_selectSfxDataId = selectSfxDataId;

				_confirmSfxDataId = confirmSfxDataId;
				_cantConfirmSfxDataId = cantConfirmSfxDataId;

				_cancelSfxDataId = cancelSfxDataId;

				_settingsShowSfxDataId = settingsShowSfxDataId;
				_settingsHideSfxDataId = settingsHideSfxDataId;

				_dialogContinueSfxDataId = dialogContinueSfxDataId;
				_dialogFunctionSfxDataId = dialogFunctionSfxDataId;
			}


			// PROTECT METHOD: --------------------------------------------------------------------

			protected virtual bool TryGetSfxDataId(string value, out string sfxDataId)
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
}