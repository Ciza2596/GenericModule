using System;
using System.Collections.Generic;
using System.Linq;
using CizaAsync;
using UnityEngine;

namespace CizaAudioModule
{
	public class BgmController
	{
		public interface IAudioPlayer
		{
			// CallerId, Id, DataId, UserId, IsOverridable, IsRecord
			event Action<string, string, string, string, bool, bool> OnBgmSpawn;
			event Action<string, string, string> OnBgmStop;

			Awaitable LoadBgmAssetAsync(string bgmDataId, string errorMessage, AsyncToken asyncToken);
			void UnloadBgmAsset(string bgmDataId);

			Awaitable<string> PlayBgmAsync(string bgmDataId, float volume = 1, float fadeTime = 0, bool isLoop = false, Vector3 position = default, bool isOverridable = false, bool isAuoDespawn = true, bool isRestrictContinuousPlay = true, bool isRecord = false, string callerId = null, AsyncToken asyncToken = default);
			Awaitable ModifyBgmAsync(string bgmId, float volume, float fadeTime = 0, AsyncToken asyncToken = default);
			Awaitable StopBgmAsync(string bgmId, float fadeTime = 0, AsyncToken asyncToken = default);
		}

		public const string CALLER_ID = "BgmController";
		public const float MIN_VOLUME = 0;

		private readonly IBgmControllerConfig _config;
		private readonly IAudioPlayer _audioPlayer;

		private readonly List<string> _loadedBgmDataIds = new List<string>();

		private readonly Dictionary<string, string> _bgmIdMapByBgmDataId = new Dictionary<string, string>();

		private string _currentBgmDataId = string.Empty;

		public string[] LoadedBgmDataIds => _loadedBgmDataIds.ToHashSet().ToArray();

		public string CurrentBgmDataId => _currentBgmDataId;


		public BgmController(IBgmControllerConfig bgmControllerConfig, IAudioPlayer audioPlayer)
		{
			_config = bgmControllerConfig;
			_audioPlayer = audioPlayer;
		}

		public async Awaitable InitializeAsync(AsyncToken asyncToken)
		{
			_audioPlayer.OnBgmSpawn += OnBgmPlay;
			_audioPlayer.OnBgmStop += OnBgmStop;

			var awaitables = new List<Awaitable>();
			foreach (var bgmDataId in _config.BgmDataIds)
				awaitables.Add(LoadBgmAssetAsync(bgmDataId, asyncToken));
			await Async.AllAsync(awaitables);
		}

		public void Release()
		{
			StopAllBgm();

			foreach (var sfxDataId in _loadedBgmDataIds)
				_audioPlayer.UnloadBgmAsset(sfxDataId);

			_audioPlayer.OnBgmSpawn -= OnBgmPlay;
			_audioPlayer.OnBgmStop -= OnBgmStop;
		}

		public async Awaitable PlayBgmAsync(IBgmSettings bgmSettings)
		{
			if (bgmSettings.TryGetBgmInfo(out var bgmDataId, out var volume))
				await PlayBgmAsync(bgmDataId, volume);
		}

		public Awaitable PlayBgmAsync(string bgmDataId) =>
			PlayBgmAsync(bgmDataId, 1);

		public Awaitable PlayBgmAsync(string bgmDataId, float volume) =>
			PlayBgmAsync(bgmDataId, volume, _config.FadeTime);

		public Awaitable PauseBgmAsync() =>
			PauseBgmAsync(_currentBgmDataId, _config.FadeTime);

		public async void StopAllBgm()
		{
			foreach (var bgmId in _bgmIdMapByBgmDataId.Values.ToArray())
				await _audioPlayer.StopBgmAsync(bgmId);
		}

		private async Awaitable PlayBgmAsync(string bgmDataId, float volume, float fadeTime)
		{
			var awaitables = new List<Awaitable>();
			if (_currentBgmDataId.CheckHasValue() && _currentBgmDataId != bgmDataId)
				awaitables.Add(PauseBgmAsync());

			_currentBgmDataId = bgmDataId;
			if (!_bgmIdMapByBgmDataId.TryGetValue(bgmDataId, out var bgmId))
				awaitables.Add(_audioPlayer.PlayBgmAsync(bgmDataId, fadeTime: fadeTime, volume: volume, isLoop: true, callerId: CALLER_ID).ToAwaitable());
			else
				awaitables.Add(_audioPlayer.ModifyBgmAsync(bgmId, volume, fadeTime));

			await Async.AllAsync(awaitables);
		}

		private async Awaitable PauseBgmAsync(string bgmDataId, float fadeTime)
		{
			if (!_bgmIdMapByBgmDataId.TryGetValue(bgmDataId, out var bgmId))
				return;

			await _audioPlayer.ModifyBgmAsync(bgmId, MIN_VOLUME, fadeTime);
		}

		private async Awaitable LoadBgmAssetAsync(string bgmDataId, AsyncToken asyncToken)
		{
			await _audioPlayer.LoadBgmAssetAsync(bgmDataId, $"Please check bgm: {bgmDataId} in BgmControllerConfig.", asyncToken);
			_loadedBgmDataIds.Add(bgmDataId);
		}

		void OnBgmPlay(string callerId, string audioId, string audioDataId, string userId, bool isOverridable, bool isRecord)
		{
			if (callerId != CALLER_ID)
				return;

			_bgmIdMapByBgmDataId.Add(audioDataId, audioId);
		}

		void OnBgmStop(string callerId, string audioId, string audioDataId)
		{
			if (callerId != CALLER_ID)
				return;

			_bgmIdMapByBgmDataId.Remove(audioDataId);
		}
	}
}