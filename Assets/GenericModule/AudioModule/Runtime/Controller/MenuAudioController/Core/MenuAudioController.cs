using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CizaAudioModule
{
    public class MenuAudioController
    {
        public interface IAudioPlayer : BgmController.IAudioPlayer, OptionSfxController.IAudioPlayer { }

        private readonly BgmController _bgmController;
        private readonly OptionSfxController _optionSfxController;

        public string[] LoadedBgmDataIds => _bgmController.LoadedBgmDataIds;

        public string CurrentBgmDataId => _bgmController.CurrentBgmDataId;


        public string[] LoadedSfxDataIds => _optionSfxController.LoadedSfxDataIds;

        public bool CanPlaySfx => _optionSfxController.IsEnable;


        public MenuAudioController(IMenuAudioControllerConfig menuAudioControllerConfig, IAudioPlayer audioPlayer)
        {
            _bgmController = new BgmController(menuAudioControllerConfig, audioPlayer);
            _optionSfxController = new OptionSfxController(menuAudioControllerConfig, audioPlayer);
        }

        public UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            var uniTasks = new List<UniTask>();
            uniTasks.Add(_bgmController.InitializeAsync(cancellationToken));
            uniTasks.Add(_optionSfxController.InitializeAsync(cancellationToken));
            return UniTask.WhenAll(uniTasks);
        }

        public void Release()
        {
            _bgmController.Release();
            _optionSfxController.Release();
        }

        #region Bgm

        public UniTask PlayBgmAsync(string bgmDataId) =>
            _bgmController.PlayBgmAsync(bgmDataId);

        public UniTask PlayBgmAsync(string bgmDataId, float volume) =>
            _bgmController.PlayBgmAsync(bgmDataId, volume);

        public UniTask PauseBgmAsync() =>
            _bgmController.PauseBgmAsync();

        #endregion

        #region OptionSfx

        public void EnableCanPlaySfx() =>
            _optionSfxController.Enable();

        public void DisableCanPlaySfx() =>
            _optionSfxController.Disable();


        public void PlaySelectSfx() =>
            _optionSfxController.PlaySelectSfx();

        public void PlayConfirmSfx() =>
            _optionSfxController.PlayConfirmSfx();


        public void PlayCantConfirmSfx() =>
            _optionSfxController.PlayCantConfirmSfx();

        public void PlayCancelSfx() =>
            _optionSfxController.PlayCancelSfx();


        public void PlaySettingsShowSfx() =>
            _optionSfxController.PlaySettingsShowSfx();

        public void PlaySettingsHideSfx() =>
            _optionSfxController.PlaySettingsHideSfx();


        public void PlayDialogContinueSfx() =>
            _optionSfxController.PlayDialogContinueSfx();

        public void PlayDialogFunctionSfx() =>
            _optionSfxController.PlayDialogFunctionSfx();

        #endregion
    }
}