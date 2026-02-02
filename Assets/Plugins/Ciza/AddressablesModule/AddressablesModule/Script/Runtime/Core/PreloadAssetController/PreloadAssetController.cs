using System;
using System.Collections.Generic;
using CizaAsync;
using UnityEngine;

namespace CizaAddressablesModule.Preload
{
	public class PreloadAssetController
	{
		private readonly IAssetProvider _assetProvider;

		private IPreloadAssetInfo[] _preloadAssetInfos;

		// BgmDataId (Address)
		public event Func<string, AsyncToken, Awaitable> OnLoadBgmAssetAsync;
		public event Action<string> OnUnloadBgmAsset;

		// SfxDataId (Address)
		public event Func<string, AsyncToken, Awaitable> OnLoadSfxAssetAsync;
		public event Action<string> OnUnloadSfxAsset;

		// VoiceDataId (Address)
		public event Func<string, AsyncToken, Awaitable> OnLoadVoiceAssetAsync;
		public event Action<string> OnUnloadVoiceAsset;

		public bool IsLoading { get; private set; }
		public bool IsLoaded { get; private set; }

		public PreloadAssetController(IAssetProvider assetProvider) =>
			_assetProvider = assetProvider;

		public async Awaitable LoadAssetAsync(IPreloadAssetInfo[] preloadAssetInfos, AsyncToken asyncToken)
		{
			if (IsLoading || IsLoaded)
				return;

			IsLoading = true;
			_preloadAssetInfos = preloadAssetInfos;

			var unitaks = new List<Awaitable>();
			foreach (var preloadAssetInfo in _preloadAssetInfos)
			{
				if (!preloadAssetInfo.IsPreLoad)
					continue;

				if (preloadAssetInfo.AssetKind == AssetKinds.Bgm && OnLoadBgmAssetAsync != null)
					unitaks.Add(OnLoadBgmAssetAsync.Invoke(preloadAssetInfo.Address, asyncToken));

				else if (preloadAssetInfo.AssetKind == AssetKinds.Sfx && OnLoadSfxAssetAsync != null)
					unitaks.Add(OnLoadSfxAssetAsync.Invoke(preloadAssetInfo.Address, asyncToken));

				else if (preloadAssetInfo.AssetKind == AssetKinds.Voice && OnLoadVoiceAssetAsync != null)
					unitaks.Add(OnLoadVoiceAssetAsync.Invoke(preloadAssetInfo.Address, asyncToken));

				else
					unitaks.Add(LoadAssetAsync(preloadAssetInfo.Address, preloadAssetInfo.AssetKind, asyncToken));
			}

			foreach (var awaitable in unitaks)
				await awaitable;

			IsLoading = false;

			IsLoaded = true;
		}

		public void UnloadAsset()
		{
			if (!IsLoaded)
				return;

			foreach (var preloadAssetInfo in _preloadAssetInfos)
			{
				if (!preloadAssetInfo.IsPreLoad)
					continue;

				if (preloadAssetInfo.AssetKind == AssetKinds.Bgm)
					OnUnloadBgmAsset?.Invoke(preloadAssetInfo.Address);

				else if (preloadAssetInfo.AssetKind == AssetKinds.Sfx)
					OnUnloadSfxAsset?.Invoke(preloadAssetInfo.Address);

				else if (preloadAssetInfo.AssetKind == AssetKinds.Voice)
					OnUnloadVoiceAsset?.Invoke(preloadAssetInfo.Address);

				else
					UnloadAsset(preloadAssetInfo.Address, preloadAssetInfo.AssetKind);
			}

			IsLoaded = false;
		}

		private async Awaitable LoadAssetAsync(string address, AssetKinds assetKind, AsyncToken asyncToken)
		{
			switch (assetKind)
			{
				case AssetKinds.GameObject:
					await _assetProvider.LoadAssetAsync<GameObject>(address, asyncToken);
					break;

				case AssetKinds.Sprite:
					await _assetProvider.LoadAssetAsync<Sprite>(address, asyncToken);
					break;

				case AssetKinds.ScriptableObject:
					await _assetProvider.LoadAssetAsync<ScriptableObject>(address, asyncToken);
					break;
			}
		}

		private void UnloadAsset(string address, AssetKinds assetKind)
		{
			switch (assetKind)
			{
				case AssetKinds.GameObject:
					_assetProvider.UnloadAsset<GameObject>(address);
					return;

				case AssetKinds.Sprite:
					_assetProvider.UnloadAsset<Sprite>(address);
					return;

				case AssetKinds.ScriptableObject:
					_assetProvider.UnloadAsset<ScriptableObject>(address);
					return;
			}
		}
	}
}