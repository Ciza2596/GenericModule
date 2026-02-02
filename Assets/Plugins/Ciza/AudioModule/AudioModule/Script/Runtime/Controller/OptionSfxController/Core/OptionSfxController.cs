using System.Collections.Generic;
using System.Linq;
using CizaAsync;
using UnityEngine;

namespace CizaAudioModule
{
	public class OptionSfxController
	{
		public interface IAudioPlayer
		{
			Awaitable LoadSfxAssetAsync(string sfxDataId, string errorMessage, AsyncToken asyncToken);
			void UnloadSfxAsset(string sfxDataId);

			Awaitable<string> PlaySfxAsync(string sfxDataId, float volume = 1, float fadeTime = 0, bool isLoop = false, Vector3 position = default, bool isAuoDespawn = true, bool isRestrictContinuousPlay = true, string callerId = null);
		}

		private readonly IOptionSfxControllerConfig _config;
		private readonly IAudioPlayer _audioPlayer;

		private readonly Dictionary<int, Player> _playerMapByIndex = new Dictionary<int, Player>();
		private readonly List<string> _loadedSfxDataIds = new List<string>();

		private const int DefaultPlayerIndex = 0;

		public string[] LoadedSfxDataIds => _loadedSfxDataIds.ToHashSet().ToArray();

		public bool IsEnable { get; private set; }

		public bool CheckIsEnable(int playerIndex) =>
			CheckIsEnable(playerIndex, false);

		public OptionSfxController(IOptionSfxControllerConfig config, IAudioPlayer audioPlayer)
		{
			_config = config;
			_audioPlayer = audioPlayer;
		}

		public async Awaitable InitializeAsync(AsyncToken asyncToken)
		{
			var awaitables = new List<Awaitable>();

			if (_config.TryGetSelectSfxDataId(out var selectSfxDataId))
				awaitables.Add(LoadSfxAssetAsync(selectSfxDataId, $"Please check selectSfx: {selectSfxDataId} in OptionSfxControllerConfig.", asyncToken));


			if (_config.TryGetConfirmSfxDataId(out var confirmSfxDataId))
				awaitables.Add(LoadSfxAssetAsync(confirmSfxDataId, $"Please check confirmSfx: {confirmSfxDataId} in OptionSfxControllerConfig.", asyncToken));

			if (_config.TryGetCantConfirmSfxDataId(out var cantConfirmSfxDataId))
				awaitables.Add(LoadSfxAssetAsync(cantConfirmSfxDataId, $"Please check cantConfirmSfx: {cantConfirmSfxDataId} in OptionSfxControllerConfig.", asyncToken));


			if (_config.TryGetCancelSfxDataId(out var cancelSfxDataId))
				awaitables.Add(LoadSfxAssetAsync(cancelSfxDataId, $"Please check cancelSfx: {cancelSfxDataId} in OptionSfxControllerConfig.", asyncToken));


			if (_config.TryGetSettingsShowSfxDataId(out var settingsShowSfxDataId))
				awaitables.Add(LoadSfxAssetAsync(settingsShowSfxDataId, $"Please check settingsShowSfx: {settingsShowSfxDataId} in OptionSfxControllerConfig.", asyncToken));

			if (_config.TryGetSettingsHideSfxDataId(out var settingsHideSfxDataId))
				awaitables.Add(LoadSfxAssetAsync(settingsHideSfxDataId, $"Please check settingsHideSfx: {settingsHideSfxDataId} in OptionSfxControllerConfig.", asyncToken));


			if (_config.TryGetDialogContinueSfxDataId(out var dialogContinueDataId))
				awaitables.Add(LoadSfxAssetAsync(dialogContinueDataId, $"Please check dialogContinue: {dialogContinueDataId} in OptionSfxControllerConfig.", asyncToken));

			if (_config.TryGetDialogFunctionSfxDataId(out var dialogFunctionDataId))
				awaitables.Add(LoadSfxAssetAsync(dialogFunctionDataId, $"Please check dialogFunction: {dialogFunctionDataId} in OptionSfxControllerConfig.", asyncToken));

			await Async.AllAsync(awaitables);
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

		public void PlaySelectSfx() =>
			PlaySelectSfx(DefaultPlayerIndex, true);


		public void PlayConfirmSfx() =>
			PlayConfirmSfx(DefaultPlayerIndex, true);

		public void PlayCantConfirmSfx() =>
			PlayCantConfirmSfx(DefaultPlayerIndex, true);


		public void PlayCancelSfx() =>
			PlayCancelSfx(DefaultPlayerIndex, true);


		public void PlaySettingsShowSfx() =>
			PlaySettingsShowSfx(DefaultPlayerIndex, true);

		public void PlaySettingsHideSfx() =>
			PlaySettingsHideSfx(DefaultPlayerIndex, true);


		public void PlayDialogContinueSfx() =>
			PlayDialogContinueSfx(DefaultPlayerIndex, true);


		public void PlayDialogFunctionSfx() =>
			PlayDialogFunctionSfx(DefaultPlayerIndex, true);

		#region Player

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

		public void PlaySelectSfx(int playerIndex) =>
			PlaySelectSfx(playerIndex, false);


		public void PlayConfirmSfx(int playerIndex) =>
			PlayConfirmSfx(playerIndex, false);

		public void PlayCantConfirmSfx(int playerIndex) =>
			PlayCantConfirmSfx(playerIndex, false);


		public void PlayCancelSfx(int playerIndex) =>
			PlayCancelSfx(playerIndex, false);


		public void PlaySettingsShowSfx(int playerIndex) =>
			PlaySettingsShowSfx(playerIndex, false);

		public void PlaySettingsHideSfx(int playerIndex) =>
			PlaySettingsHideSfx(playerIndex, false);


		public void PlayDialogContinueSfx(int playerIndex) =>
			PlayDialogContinueSfx(playerIndex, false);


		public void PlayDialogFunctionSfx(int playerIndex) =>
			PlayDialogFunctionSfx(playerIndex, false);

		#endregion

		private void PlaySelectSfx(int playerIndex, bool isIgnorePlayer)
		{
			if (_config.TryGetSelectSfxDataId(out var selectSfxDataId))
				PlaySfx(playerIndex, isIgnorePlayer, selectSfxDataId, "PlaySelectSfx");
		}


		private void PlayConfirmSfx(int playerIndex, bool isIgnorePlayer)
		{
			if (_config.TryGetConfirmSfxDataId(out var confirmSfxDataId))
				PlaySfx(playerIndex, isIgnorePlayer, confirmSfxDataId, "PlayConfirmSfx");
		}

		private void PlayCantConfirmSfx(int playerIndex, bool isIgnorePlayer)
		{
			if (_config.TryGetCantConfirmSfxDataId(out var cantConfirmSfxDataId))
				PlaySfx(playerIndex, isIgnorePlayer, cantConfirmSfxDataId, "CantConfirmSfx");
		}


		private void PlayCancelSfx(int playerIndex, bool isIgnorePlayer)
		{
			if (_config.TryGetCancelSfxDataId(out var cancelSfxDataId))
				PlaySfx(playerIndex, isIgnorePlayer, cancelSfxDataId, "PlayCancelSfx");
		}


		private void PlaySettingsShowSfx(int playerIndex, bool isIgnorePlayer)
		{
			if (_config.TryGetSettingsShowSfxDataId(out var settingsShowSfxDataId))
				PlaySfx(playerIndex, isIgnorePlayer, settingsShowSfxDataId, "PlaySettingsShowSfx");
		}

		private void PlaySettingsHideSfx(int playerIndex, bool isIgnorePlayer)
		{
			if (_config.TryGetSettingsHideSfxDataId(out var settingsHideSfxDataId))
				PlaySfx(playerIndex, isIgnorePlayer, settingsHideSfxDataId, "PlaySettingsHideSfx");
		}

		private void PlayDialogContinueSfx(int playerIndex, bool isIgnorePlayer)
		{
			if (_config.TryGetDialogContinueSfxDataId(out var dialogContinueSfxDataId))
				PlaySfx(playerIndex, isIgnorePlayer, dialogContinueSfxDataId, "PlayDialogContinueSfx");
		}

		private void PlayDialogFunctionSfx(int playerIndex, bool isIgnorePlayer)
		{
			if (_config.TryGetDialogFunctionSfxDataId(out var dialogFunctionSfxDataId))
				PlaySfx(playerIndex, isIgnorePlayer, dialogFunctionSfxDataId, "PlayDialogFunctionSfx");
		}

		private async void PlaySfx(int playerIndex, bool isIgnorePlayer, string sfxDataId, string methodName)
		{
			if (CheckIsEnable(playerIndex, isIgnorePlayer) && CheckIsLoadedSfx(sfxDataId, methodName))
				await _audioPlayer.PlaySfxAsync(sfxDataId);
		}

		private bool CheckIsEnable(int playerIndex, bool isIgnorePlayer)
		{
			if (isIgnorePlayer)
				return IsEnable;

			return IsEnable && _playerMapByIndex.TryGetValue(playerIndex, out var player) && player.IsEnable;
		}

		private bool CheckIsLoadedSfx(string sfxDataId, string methodName)
		{
			if (_loadedSfxDataIds.Contains(sfxDataId))
				return true;

			Debug.LogWarning($"[OptionSfxController::{methodName}] Sfx: {sfxDataId} is not loaded.");
			return false;
		}

		private async Awaitable LoadSfxAssetAsync(string sfxDataId, string errorMessage, AsyncToken asyncToken)
		{
			await _audioPlayer.LoadSfxAssetAsync(sfxDataId, errorMessage, asyncToken);
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