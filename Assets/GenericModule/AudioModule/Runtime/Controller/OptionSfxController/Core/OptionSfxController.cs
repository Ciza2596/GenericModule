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

        private readonly Dictionary<int, Player> _playerMapByIndex = new Dictionary<int, Player>();
        private readonly List<string> _loadedSfxDataIds = new List<string>();

        public string[] LoadedSfxDataIds => _loadedSfxDataIds.ToHashSet().ToArray();

        public bool IsEnable { get; private set; }

        public bool CheckIsEnable(int playerIndex) =>
            IsEnable && _playerMapByIndex.TryGetValue(playerIndex, out var player) && player.IsEnable;


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

        public void Enable(int playerIndex)
        {
            if (!_playerMapByIndex.TryGetValue(playerIndex, out var player))
                return;

            player.Enable();
        }

        public void Disable(int playerIndex)
        {
            if (!_playerMapByIndex.TryGetValue(playerIndex, out var player))
                return;

            player.Disable();
        }

        public void ResetPlayerCount(int playerCount)
        {
            _playerMapByIndex.Clear();

            for (var i = 0; i < playerCount; i++)
                AddPlayer(i);
        }

        public void AddPlayer(int playerIndex)
        {
            if (_playerMapByIndex.ContainsKey(playerIndex))
                return;

            _playerMapByIndex.Add(playerIndex, new Player(playerIndex));
        }

        public void RemovePlayer(int playerIndex)
        {
            if (!_playerMapByIndex.ContainsKey(playerIndex))
                return;

            _playerMapByIndex.Remove(playerIndex);
        }

        public void PlaySelectSfx(int playerIndex)
        {
            if (_config.TryGetSelectSfxDataId(out var selectSfxDataId))
                PlaySfx(playerIndex, selectSfxDataId, "PlaySelectSfx");
        }


        public void PlayConfirmSfx(int playerIndex)
        {
            if (_config.TryGetConfirmSfxDataId(out var confirmSfxDataId))
                PlaySfx(playerIndex, confirmSfxDataId, "PlayConfirmSfx");
        }

        public void PlayCantConfirmSfx(int playerIndex)
        {
            if (_config.TryGetCantConfirmSfxDataId(out var cantConfirmSfxDataId))
                PlaySfx(playerIndex, cantConfirmSfxDataId, "CantConfirmSfx");
        }


        public void PlayCancelSfx(int playerIndex)
        {
            if (_config.TryGetCancelSfxDataId(out var cancelSfxDataId))
                PlaySfx(playerIndex, cancelSfxDataId, "PlayCancelSfx");
        }


        public void PlaySettingsShowSfx(int playerIndex)
        {
            if (_config.TryGetSettingsShowSfxDataId(out var settingsShowSfxDataId))
                PlaySfx(playerIndex, settingsShowSfxDataId, "PlaySettingsShowSfx");
        }

        public void PlaySettingsHideSfx(int playerIndex)
        {
            if (_config.TryGetSettingsHideSfxDataId(out var settingsHideSfxDataId))
                PlaySfx(playerIndex, settingsHideSfxDataId, "PlaySettingsHideSfx");
        }

        public void PlayDialogContinueSfx(int playerIndex)
        {
            if (_config.TryGetDialogContinueSfxDataId(out var dialogContinueSfxDataId))
                PlaySfx(playerIndex, dialogContinueSfxDataId, "PlayDialogContinueSfx");
        }

        public void PlayDialogFunctionSfx(int playerIndex)
        {
            if (_config.TryGetDialogFunctionSfxDataId(out var dialogFunctionSfxDataId))
                PlaySfx(playerIndex, dialogFunctionSfxDataId, "PlayDialogFunctionSfx");
        }

        private async void PlaySfx(int playerIndex, string sfxDataId, string methodName)
        {
            if (CheckIsEnable(playerIndex) && CheckIsLoadedSfx(sfxDataId, methodName))
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

        private class Player
        {
            public int Index { get; private set; }
            public bool IsEnable { get; private set; }

            public Player(int index)
            {
                Index = index;
                Enable();
            }

            public void Enable() =>
                SetIsEnable(true);

            public void Disable() =>
                SetIsEnable(false);

            private void SetIsEnable(bool isEnable) =>
                IsEnable = isEnable;
        }
    }
}