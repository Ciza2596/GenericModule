namespace CizaAudioModule
{
    public interface IOptionSfxControllerConfig
    {
        bool TryGetSelectSfxDataId(out string selectSfxDataId);

        bool TryGetConfirmSfxDataId(out string confirmSfxDataId);
        bool TryGetCantConfirmSfxDataId(out string cantConfirmSfxDataId);

        bool TryGetCancelSfxDataId(out string cancelSfxDataId);

        bool TryGetSettingsShowSfxDataId(out string settingsShowSfxDataId);
        bool TryGetSettingsHideSfxDataId(out string settingsHideSfxDataId);

        bool TryGetDialogContinueSfxDataId(out string dialogContinueSfxDataId);
        bool TryGetDialogFunctionSfxDataId(out string dialogFunctionSfxDataId);
    }
}