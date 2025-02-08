using UnityEngine;

namespace CizaAudioModule.Implement
{
    [CreateAssetMenu(fileName = "OptionSfxControllerConfig", menuName = "Ciza/AudioModule/OptionSfxControllerConfig", order = 201)]
    public class OptionSfxControllerConfig : ScriptableObject, IOptionSfxControllerConfig
    {
        [SerializeField]
        private string _selectSfxDataId = "UI_Select";

        [Space]
        [SerializeField]
        private string _confirmSfxDataId= "UI_Confirm";

        [SerializeField]
        private string _cantConfirmSfxDataId= "UI_CantConfirm";

        [Space]
        [SerializeField]
        private string _cancelSfxDataId= "UI_Cancel";

        [Space]
        [SerializeField]
        private string _settingsShowSfxDataId= "UI_SettingsShow";

        [SerializeField]
        private string _settingsHideSfxDataId= "UI_SettingsHide";

        [Space]
        [SerializeField]
        private string _dialogContinueSfxDataId= "UI_DialogContinue";

        [SerializeField]
        private string _dialogFunctionSfxDataId= "UI_DialogFunction";


        public bool TryGetSelectSfxDataId(out string selectSfxDataId) =>
            TryGetSfxDataId(_selectSfxDataId, out selectSfxDataId);


        public bool TryGetConfirmSfxDataId(out string confirmSfxDataId) =>
            TryGetSfxDataId(_confirmSfxDataId, out confirmSfxDataId);

        public bool TryGetCantConfirmSfxDataId(out string cantConfirmSfxDataId) =>
            TryGetSfxDataId(_cantConfirmSfxDataId, out cantConfirmSfxDataId);


        public bool TryGetCancelSfxDataId(out string cancelSfxDataId) =>
            TryGetSfxDataId(_cancelSfxDataId, out cancelSfxDataId);


        public bool TryGetSettingsShowSfxDataId(out string settingsShowSfxDataId) =>
            TryGetSfxDataId(_settingsShowSfxDataId, out settingsShowSfxDataId);

        public bool TryGetSettingsHideSfxDataId(out string settingsHideSfxDataId) =>
            TryGetSfxDataId(_settingsHideSfxDataId, out settingsHideSfxDataId);


        public bool TryGetDialogContinueSfxDataId(out string dialogContinueSfxDataId) =>
            TryGetSfxDataId(_dialogContinueSfxDataId, out dialogContinueSfxDataId);

        public bool TryGetDialogFunctionSfxDataId(out string dialogFunctionSfxDataId) =>
            TryGetSfxDataId(_dialogFunctionSfxDataId, out dialogFunctionSfxDataId);


        private bool TryGetSfxDataId(string value, out string sfxDataId)
        {
            if (!value.HasValue())
            {
                sfxDataId = string.Empty;
                return false;
            }

            sfxDataId = value;
            return true;
        }
    }
}