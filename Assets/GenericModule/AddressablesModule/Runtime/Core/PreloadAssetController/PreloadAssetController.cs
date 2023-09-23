using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaPreloadAssetController
{
	public class PreloadAssetController
	{
		private readonly IAssetProvider _assetProvider;

		private IPreloadAssetInfo[] _preloadAssetInfos;

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

				unitaks.Add(LoadAssetAsync(preloadAssetInfo.Address, preloadAssetInfo.AssetType, cancellationToken));
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

				UnloadAsset(preloadAssetInfo.Address, preloadAssetInfo.AssetType);
			}

			IsLoaded = false;
		}

		private UniTask LoadAssetAsync(string address, AssetTypes assetType, CancellationToken cancellationToken)
		{
			switch (assetType)
			{
				case AssetTypes.GameObject:
					return _assetProvider.LoadAssetAsync<GameObject>(address, cancellationToken);

				case AssetTypes.Sprite:
					return _assetProvider.LoadAssetAsync<Sprite>(address, cancellationToken);

				case AssetTypes.ScriptableObject:
					return _assetProvider.LoadAssetAsync<ScriptableObject>(address, cancellationToken);
			}

			return UniTask.CompletedTask;
		}

		private void UnloadAsset(string address, AssetTypes assetType)
		{
			switch (assetType)
			{
				case AssetTypes.GameObject:
					_assetProvider.UnloadAsset<GameObject>(address);
					return;

				case AssetTypes.Sprite:
					_assetProvider.UnloadAsset<Sprite>(address);
					return;

				case AssetTypes.ScriptableObject:
					_assetProvider.UnloadAsset<ScriptableObject>(address);
					return;
			}
		}
	}
}
