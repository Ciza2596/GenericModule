using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CizaUniTask;
using UnityEngine;

namespace CizaAudioModule
{
	public class BgmController
	{
		public interface IAudioPlayer
		{
			event Action<string, string, string> OnBgmSpawn;
			event Action<string, string, string> OnBgmStop;

			UniTask LoadBgmAssetAsync(string bgmDataId, string errorMessage, CancellationToken cancellationToken);
			void UnloadBgmAsset(string bgmDataId);

			UniTask<string> PlayBgmAsync(string bgmDataId, float volume = 1, float fadeTime = 0, bool isLoop = false, Vector3 position = default, bool isAuoDespawn = true, bool isRestrictContinuousPlay = true, string callerId = null);
			UniTask ModifyBgmAsync(string bgmId, float volume, float time = 0);
			UniTask StopBgmAsync(string bgmId, float fadeTime = 0);
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

		public UniTask InitializeAsync(CancellationToken cancellationToken)
		{
			_audioPlayer.OnBgmSpawn += OnBgmPlay;
			_audioPlayer.OnBgmStop += OnBgmStop;

			var uniTasks = new List<UniTask>();
			foreach (var bgmDataId in _config.BgmDataIds)
				uniTasks.Add(LoadBgmAssetAsync(bgmDataId, cancellationToken));

			return UniTask.WhenAll(uniTasks);
		}

		public void Release()
		{
			StopAllBgm();

			foreach (var sfxDataId in _loadedBgmDataIds)
				_audioPlayer.UnloadBgmAsset(sfxDataId);

			_audioPlayer.OnBgmSpawn -= OnBgmPlay;
			_audioPlayer.OnBgmStop -= OnBgmStop;
		}

		public UniTask PlayBgmAsync(IBgmSettings bgmSettings)
		{
			if (bgmSettings.TryGetBgmInfo(out var bgmDataId, out var volume))
				return PlayBgmAsync(bgmDataId, volume);

			return UniTask.CompletedTask;
		}

		public UniTask PlayBgmAsync(string bgmDataId) =>
			PlayBgmAsync(bgmDataId, 1);

		public UniTask PlayBgmAsync(string bgmDataId, float volume) =>
			PlayBgmAsync(bgmDataId, volume, _config.FadeTime);

		public UniTask PauseBgmAsync() =>
			PauseBgmAsync(_currentBgmDataId, _config.FadeTime);

		public async void StopAllBgm()
		{
			foreach (var bgmId in _bgmIdMapByBgmDataId.Values.ToArray())
				await _audioPlayer.StopBgmAsync(bgmId);
		}

		private UniTask PlayBgmAsync(string bgmDataId, float volume, float fadeTime)
		{
			var uniTasks = new List<UniTask>();

			if (_currentBgmDataId.CheckHasValue() && _currentBgmDataId != bgmDataId)
				uniTasks.Add(PauseBgmAsync());

			_currentBgmDataId = bgmDataId;
			if (!_bgmIdMapByBgmDataId.TryGetValue(bgmDataId, out var bgmId))
				uniTasks.Add(_audioPlayer.PlayBgmAsync(bgmDataId, fadeTime: fadeTime, volume: volume, isLoop: true, callerId: CallerId));
			else
				uniTasks.Add(_audioPlayer.ModifyBgmAsync(bgmId, volume, fadeTime));

			return UniTask.WhenAll(uniTasks);
		}

		private UniTask PauseBgmAsync(string bgmDataId, float fadeTime)
		{
			if (!_bgmIdMapByBgmDataId.TryGetValue(bgmDataId, out var bgmId))
				return UniTask.CompletedTask;

			return _audioPlayer.ModifyBgmAsync(bgmId, MinVolume, fadeTime);
		}

		private async UniTask LoadBgmAssetAsync(string bgmDataId, CancellationToken cancellationToken)
		{
			await _audioPlayer.LoadBgmAssetAsync(bgmDataId, $"Please check bgm: {bgmDataId} in BgmControllerConfig.", cancellationToken);
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