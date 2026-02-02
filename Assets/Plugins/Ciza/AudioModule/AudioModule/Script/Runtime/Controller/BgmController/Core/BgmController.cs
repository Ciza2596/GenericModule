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
			event Action<string, string, string> OnBgmSpawn;
			event Action<string, string, string> OnBgmStop;

			Awaitable LoadBgmAssetAsync(string bgmDataId, string errorMessage, AsyncToken asyncToken);
			void UnloadBgmAsset(string bgmDataId);

			Awaitable<string> PlayBgmAsync(string bgmDataId, float volume = 1, float fadeTime = 0, bool isLoop = false, Vector3 position = default, bool isAuoDespawn = true, bool isRestrictContinuousPlay = true, string callerId = null);
			Awaitable ModifyBgmAsync(string bgmId, float volume, float time = 0);
			Awaitable StopBgmAsync(string bgmId, float fadeTime = 0);
		}

		public const string CallerId = "BgmController";
		public const float MinVolume = 0;

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
				awaitables.Add(_audioPlayer.PlayBgmAsync(bgmDataId, fadeTime: fadeTime, volume: volume, isLoop: true, callerId: CallerId).ToAwaitable());
			else
				awaitables.Add(_audioPlayer.ModifyBgmAsync(bgmId, volume, fadeTime));

			await Async.AllAsync(awaitables);
		}

		private async Awaitable PauseBgmAsync(string bgmDataId, float fadeTime)
		{
			if (!_bgmIdMapByBgmDataId.TryGetValue(bgmDataId, out var bgmId))
				return;

			await _audioPlayer.ModifyBgmAsync(bgmId, MinVolume, fadeTime);
		}

		private async Awaitable LoadBgmAssetAsync(string bgmDataId, AsyncToken asyncToken)
		{
			await _audioPlayer.LoadBgmAssetAsync(bgmDataId, $"Please check bgm: {bgmDataId} in BgmControllerConfig.", asyncToken);
			_loadedBgmDataIds.Add(bgmDataId);
		}

		void OnBgmPlay(string callerId, string audioId, string audioDataId)
		{
			if (callerId != CallerId)
				return;

			_bgmIdMapByBgmDataId.Add(audioDataId, audioId);
		}

		void OnBgmStop(string callerId, string audioId, string audioDataId)
		{
			if (callerId != CallerId)
				return;

			_bgmIdMapByBgmDataId.Remove(audioDataId);
		}
	}
}