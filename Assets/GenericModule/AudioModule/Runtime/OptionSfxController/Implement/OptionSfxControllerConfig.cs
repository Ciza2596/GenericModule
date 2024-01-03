using UnityEngine;

namespace CizaAudioModule.Implement
{
    [CreateAssetMenu(fileName = "OptionSfxControllerConfig", menuName = "Ciza/AudioModule/OptionSfxControllerConfig")]
    public class OptionSfxControllerConfig : ScriptableObject, IOptionSfxControllerConfig
    {
        [SerializeField]
        private string _selectSfxDataId;

        [Space]
        [SerializeField]
        private string _confirmSfxDataId;

        [SerializeField]
        private string _cantConfirmSfxDataId;

        [Space]
        [SerializeField]
        private string _cancelSfxDataId;

        [Space]
        [SerializeField]
        private string _settingsShowSfxDataId;

        [SerializeField]
        private string _settingsHideSfxDataId;

        [Space]
        [SerializeField]
        private string _dialogContinueSfxDataId;

        [SerializeField]
        private string _dialogFunctionSfxDataId;


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
            if (!CheckHasValue(value))
            {
                sfxDataId = string.Empty;
                return false;
            }

            sfxDataId = value;
            return true;
        }

        private bool CheckHasValue(string value) =>
            !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value);
    }
}