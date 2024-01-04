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
        public bool CheckCanPlaySfx(int playerIndex) => _optionSfxController.CheckIsEnable(playerIndex);


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

        public UniTask PlayBgmAsync(IBgmSettings bgmSettings) =>
            _bgmController.PlayBgmAsync(bgmSettings);

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


        public void EnableCanPlaySfx(int playerIndex) =>
            _optionSfxController.Enable(playerIndex);

        public void DisableCanPlaySfx(int playerIndex) =>
            _optionSfxController.Disable(playerIndex);


        public void ResetPlayerCount(int playerCount) =>
            _optionSfxController.ResetPlayerCount(playerCount);

        public void AddPlayer(int playerIndex) =>
            _optionSfxController.AddPlayer(playerIndex);

        public void RemovePlayer(int playerIndex) =>
            _optionSfxController.RemovePlayer(playerIndex);


        public void PlaySelectSfx(int playerIndex) =>
            _optionSfxController.PlaySelectSfx(playerIndex);

        public void PlayConfirmSfx(int playerIndex) =>
            _optionSfxController.PlayConfirmSfx(playerIndex);


        public void PlayCantConfirmSfx(int playerIndex) =>
            _optionSfxController.PlayCantConfirmSfx(playerIndex);

        public void PlayCancelSfx(int playerIndex) =>
            _optionSfxController.PlayCancelSfx(playerIndex);


        public void PlaySettingsShowSfx(int playerIndex) =>
            _optionSfxController.PlaySettingsShowSfx(playerIndex);

        public void PlaySettingsHideSfx(int playerIndex) =>
            _optionSfxController.PlaySettingsHideSfx(playerIndex);


        public void PlayDialogContinueSfx(int playerIndex) =>
            _optionSfxController.PlayDialogContinueSfx(playerIndex);

        public void PlayDialogFunctionSfx(int playerIndex) =>
            _optionSfxController.PlayDialogFunctionSfx(playerIndex);

        #endregion
    }
}