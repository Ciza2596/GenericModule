using System;
using System.Linq;
using UnityEngine;

namespace CizaAudioModule.Implement
{
    [CreateAssetMenu(fileName = "MenuAudioControllerConfig", menuName = "Ciza/AudioModule/MenuAudioControllerConfig", order = 100)]
    public class MenuAudioControllerConfig : ScriptableObject, IMenuAudioControllerConfig
    {
        [SerializeField]
        private BgmSettings _bgmSettings;

        [Space]
        [SerializeField]
        private OptionSfxSettings _optionSfxSettings;


        public float FadeTime => _bgmSettings.FadeTime;
        public string[] BgmDataIds => _bgmSettings.BgmDataIds;


        public bool TryGetSelectSfxDataId(out string selectSfxDataId) =>
            _optionSfxSettings.TryGetSelectSfxDataId(out selectSfxDataId);


        public bool TryGetConfirmSfxDataId(out string confirmSfxDataId) =>
            _optionSfxSettings.TryGetConfirmSfxDataId(out confirmSfxDataId);

        public bool TryGetCantConfirmSfxDataId(out string cantConfirmSfxDataId) =>
            _optionSfxSettings.TryGetCantConfirmSfxDataId(out cantConfirmSfxDataId);


        public bool TryGetCancelSfxDataId(out string cancelSfxDataId) =>
            _optionSfxSettings.TryGetCancelSfxDataId(out cancelSfxDataId);


        public bool TryGetSettingsShowSfxDataId(out string settingsShowSfxDataId) =>
            _optionSfxSettings.TryGetSettingsShowSfxDataId(out settingsShowSfxDataId);

        public bool TryGetSettingsHideSfxDataId(out string settingsHideSfxDataId) =>
            _optionSfxSettings.TryGetSettingsHideSfxDataId(out settingsHideSfxDataId);


        public bool TryGetDialogContinueSfxDataId(out string dialogContinueSfxDataId) =>
            _optionSfxSettings.TryGetDialogContinueSfxDataId(out dialogContinueSfxDataId);

        public bool TryGetDialogFunctionSfxDataId(out string dialogFunctionSfxDataId) =>
            _optionSfxSettings.TryGetDialogFunctionSfxDataId(out dialogFunctionSfxDataId);

        [Serializable]
        private class BgmSettings
        {
            [SerializeField]
            private float _fadeTime = 0.25f;

            [Space]
            [SerializeField]
            private string[] _bgmDataIds;

            public float FadeTime => _fadeTime;

            public string[] BgmDataIds => _bgmDataIds != null ? _bgmDataIds.ToHashSet().ToArray() : Array.Empty<string>();
        }

        [Serializable]
        private class OptionSfxSettings
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
}