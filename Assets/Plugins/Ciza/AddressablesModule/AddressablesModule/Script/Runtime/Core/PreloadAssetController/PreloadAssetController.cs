using System;
using System.Collections.Generic;
using System.Threading;
using CizaUniTask;
using UnityEngine;

namespace CizaAddressablesModule.Preload
{
	public class PreloadAssetController
	{
		private readonly IAssetProvider _assetProvider;

		private IPreloadAssetInfo[] _preloadAssetInfos;

		// BgmDataId (Address)
		public event Func<string, CancellationToken, UniTask> OnLoadBgmAssetAsync;
		public event Action<string>                           OnUnloadBgmAsset;

		// SfxDataId (Address)
		public event Func<string, CancellationToken, UniTask> OnLoadSfxAssetAsync;
		public event Action<string>                           OnUnloadSfxAsset;

		// VoiceDataId (Address)
		public event Func<string, CancellationToken, UniTask> OnLoadVoiceAssetAsync;
		public event Action<string>                           OnUnloadVoiceAsset;

		public bool IsLoading { get; private set; }
		public bool IsLoaded  { get; private set; }

		public PreloadAssetController(IAssetProvider assetProvider) =>
			_assetProvider = assetProvider;

		public async UniTask LoadAssetAsync(IPreloadAssetInfo[] preloadAssetInfos, CancellationToken cancellationToken)
		{
			if (IsLoading || IsLoaded)
				return;

			IsLoading          = true;
			_preloadAssetInfos = preloadAssetInfos;

			var unitaks = new List<UniTask>();
			foreach (var preloadAssetInfo in _preloadAssetInfos)
			{
				if (!preloadAssetInfo.IsPreLoad)
					continue;

				if (preloadAssetInfo.AssetKind == AssetKinds.Bgm && OnLoadBgmAssetAsync != null)
					unitaks.Add(OnLoadBgmAssetAsync.Invoke(preloadAssetInfo.Address, cancellationToken));

				else if (preloadAssetInfo.AssetKind == AssetKinds.Sfx && OnLoadSfxAssetAsync != null)
					unitaks.Add(OnLoadSfxAssetAsync.Invoke(preloadAssetInfo.Address, cancellationToken));

				else if (preloadAssetInfo.AssetKind == AssetKinds.Voice && OnLoadVoiceAssetAsync != null)
					unitaks.Add(OnLoadVoiceAssetAsync.Invoke(preloadAssetInfo.Address, cancellationToken));

				else
					unitaks.Add(LoadAssetAsync(preloadAssetInfo.Address, preloadAssetInfo.AssetKind, cancellationToken));
			}

			await UniTask.WhenAll(unitaks);
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

		private UniTask LoadAssetAsync(string address, AssetKinds assetKind, CancellationToken cancellationToken)
		{
			switch (assetKind)
			{
				case AssetKinds.GameObject:
					return _assetProvider.LoadAssetAsync<GameObject>(address, cancellationToken);

				case AssetKinds.Sprite:
					return _assetProvider.LoadAssetAsync<Sprite>(address, cancellationToken);

				case AssetKinds.ScriptableObject:
					return _assetProvider.LoadAssetAsync<ScriptableObject>(address, cancellationToken);
			}

			return UniTask.CompletedTask;
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
