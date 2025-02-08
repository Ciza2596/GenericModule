using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaAddressablesModule.Preload
{
	public interface IAssetProvider
	{
		UniTask<T> LoadAssetAsync<T>(string address, CancellationToken cancellationToken) where T : Object;

		void UnloadAsset<T>(string address) where T : Object;
	}
}
