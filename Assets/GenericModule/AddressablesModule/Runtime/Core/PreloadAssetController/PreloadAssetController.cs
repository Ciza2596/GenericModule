using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaAddressablesModule.PreloadAssetController
{
	public class PreloadAssetController
	{
		private readonly IAssetProvider _assetProvider;

		private IPreloadAssetInfo[] _preloadAssetInfos;

		// BgmDataId (Address)
		public event Func<string, CancellationToken, UniTask> OnLoadBgm;
		public event Action<string>                           OnUnloadBgm;

		// SfxDataId (Address)
		public event Func<string, CancellationToken, UniTask> OnLoadSfx;
		public event Action<string>                           OnUnloadSfx;

		// VoiceDataId (Address)
		public event Func<string, CancellationToken, UniTask> OnLoadVoice;
		public event Action<string>                           OnUnloadVoice;

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

				if (preloadAssetInfo.AssetKind == AssetKinds.Bgm && OnLoadBgm != null)
					unitaks.Add(OnLoadBgm.Invoke(preloadAssetInfo.Address, cancellationToken));

				else if (preloadAssetInfo.AssetKind == AssetKinds.Sfx && OnLoadSfx != null)
					unitaks.Add(OnLoadSfx.Invoke(preloadAssetInfo.Address, cancellationToken));

				else if (preloadAssetInfo.AssetKind == AssetKinds.Voice && OnLoadVoice != null)
					unitaks.Add(OnLoadVoice.Invoke(preloadAssetInfo.Address, cancellationToken));

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
					OnUnloadBgm?.Invoke(preloadAssetInfo.Address);

				else if (preloadAssetInfo.AssetKind == AssetKinds.Sfx)
					OnUnloadSfx?.Invoke(preloadAssetInfo.Address);

				else if (preloadAssetInfo.AssetKind == AssetKinds.Voice)
					OnUnloadVoice?.Invoke(preloadAssetInfo.Address);

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
