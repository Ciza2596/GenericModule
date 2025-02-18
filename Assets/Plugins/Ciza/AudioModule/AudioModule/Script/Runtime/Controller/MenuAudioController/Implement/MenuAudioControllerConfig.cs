using System;
using System.Linq;
using UnityEngine;

namespace CizaAudioModule.Implement
{
	[CreateAssetMenu(fileName = "MenuAudioControllerConfig", menuName = "Ciza/AudioModule/MenuAudioControllerConfig", order = 101)]
	public class MenuAudioControllerConfig : ScriptableObject, IMenuAudioControllerConfig
	{
		[SerializeField]
		protected BgmSettings _bgmSettings;

		[Space]
		[SerializeField]
		private OptionSfxSettings _optionSfxSettings;


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

		[Serializable]
		protected class BgmSettings : IZomeraphyPanel
		{
			[SerializeField]
			protected float _fadeTime = 0.5f;

			[Space]
			[SerializeField]
			[OverrideDrawer]
			protected string[] _bgmDataIds;

			public virtual float FadeTime => _fadeTime;

			public virtual string[] BgmDataIds => _bgmDataIds != null ? _bgmDataIds.ToHashSet().ToArray() : Array.Empty<string>();
		}

		[Serializable]
		protected class OptionSfxSettings : IZomeraphyPanel
		{
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