using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaAudioModule
{
    public class OptionSfxController
    {
        public interface IAudioPlayer
        {
            UniTask LoadSfxAssetAsync(string sfxDataId, CancellationToken cancellationToken);
            void UnloadSfxAsset(string sfxDataId);

            UniTask<string> PlaySfxAsync(string sfxDataId, float volume = 1, float fadeTime = 0, bool isLoop = false, Vector3 position = default, string callerId = null);
        }

        private readonly IOptionSfxControllerConfig _config;
        private readonly IAudioPlayer _audioPlayer;

        private readonly List<string> _loadedSfxDataIds = new List<string>();

        public string[] LoadedSfxDataIds => _loadedSfxDataIds.ToHashSet().ToArray();

        public bool IsEnable { get; private set; }

        public OptionSfxController(IOptionSfxControllerConfig config, IAudioPlayer audioPlayer)
        {
            _config = config;
            _audioPlayer = audioPlayer;
        }

        public UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            var uniTasks = new List<UniTask>();

            if (_config.TryGetSelectSfxDataId(out var selectSfxDataId))
                uniTasks.Add(LoadSfxAssetAsync(selectSfxDataId, cancellationToken));


            if (_config.TryGetConfirmSfxDataId(out var confirmSfxDataId))
                uniTasks.Add(LoadSfxAssetAsync(confirmSfxDataId, cancellationToken));

            if (_config.TryGetCantConfirmSfxDataId(out var cantConfirmSfxDataId))
                uniTasks.Add(LoadSfxAssetAsync(cantConfirmSfxDataId, cancellationToken));


            if (_config.TryGetCancelSfxDataId(out var cancelSfxDataId))
                uniTasks.Add(LoadSfxAssetAsync(cancelSfxDataId, cancellationToken));


            if (_config.TryGetSettingsShowSfxDataId(out var settingsShowSfxDataId))
                uniTasks.Add(LoadSfxAssetAsync(settingsShowSfxDataId, cancellationToken));

            if (_config.TryGetSettingsHideSfxDataId(out var settingsHideSfxDataId))
                uniTasks.Add(LoadSfxAssetAsync(settingsHideSfxDataId, cancellationToken));


            if (_config.TryGetDialogContinueSfxDataId(out var dialogContinueDataId))
                uniTasks.Add(LoadSfxAssetAsync(dialogContinueDataId, cancellationToken));

            if (_config.TryGetDialogFunctionSfxDataId(out var dialogFunctionDataId))
                uniTasks.Add(LoadSfxAssetAsync(dialogFunctionDataId, cancellationToken));

            return UniTask.WhenAll(uniTasks);
        }

        public void Release()
        {
            foreach (var sfxDataId in _loadedSfxDataIds)
                _audioPlayer.UnloadSfxAsset(sfxDataId);
        }

        public void Enable() =>
            IsEnable = true;

        public void Disable() =>
            IsEnable = false;


        public void PlaySelectSfx()
        {
            if (_config.TryGetSelectSfxDataId(out var selectSfxDataId))
                PlaySfx(selectSfxDataId, "PlaySelectSfx");
        }


        public void PlayConfirmSfx()
        {
            if (_config.TryGetConfirmSfxDataId(out var confirmSfxDataId))
                PlaySfx(confirmSfxDataId, "PlayConfirmSfx");
        }

        public void PlayCantConfirmSfx()
        {
            if (_config.TryGetCantConfirmSfxDataId(out var cantConfirmSfxDataId))
                PlaySfx(cantConfirmSfxDataId, "CantConfirmSfx");
        }


        public void PlayCancelSfx()
        {
            if (_config.TryGetCancelSfxDataId(out var cancelSfxDataId))
                PlaySfx(cancelSfxDataId, "PlayCancelSfx");
        }


        public void PlaySettingsShowSfx()
        {
            if (_config.TryGetSettingsShowSfxDataId(out var settingsShowSfxDataId))
                PlaySfx(settingsShowSfxDataId, "PlaySettingsShowSfx");
        }

        public void PlaySettingsHideSfx()
        {
            if (_config.TryGetSettingsHideSfxDataId(out var settingsHideSfxDataId))
                PlaySfx(settingsHideSfxDataId, "PlaySettingsHideSfx");
        }


        public void PlayDialogContinueSfx()
        {
            if (_config.TryGetDialogContinueSfxDataId(out var dialogContinueSfxDataId))
                PlaySfx(dialogContinueSfxDataId, "PlayDialogContinueSfx");
        }

        public void PlayDialogFunctionSfx()
        {
            if (_config.TryGetDialogFunctionSfxDataId(out var dialogFunctionSfxDataId))
                PlaySfx(dialogFunctionSfxDataId, "PlayDialogFunctionSfx");
        }

        private async void PlaySfx(string sfxDataId, string methodName)
        {
            if (IsEnable && CheckIsLoadedSfx(sfxDataId, methodName))
                await _audioPlayer.PlaySfxAsync(sfxDataId);
        }

        private bool CheckIsLoadedSfx(string sfxDataId, string methodName)
        {
            if (_loadedSfxDataIds.Contains(sfxDataId))
                return true;

            Debug.LogWarning($"[OptionSfxController::{methodName}] Sfx: {sfxDataId} is not loaded.");
            return false;
        }
        
        private async UniTask LoadSfxAssetAsync(string sfxDataId, CancellationToken cancellationToken)
        {
            await _audioPlayer.LoadSfxAssetAsync(sfxDataId, cancellationToken);
            _loadedSfxDataIds.Add(sfxDataId);
        }
    }
}