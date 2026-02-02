using CizaAsync;
using UnityEngine;

namespace CizaAddressablesModule.Preload
{
	public interface IAssetProvider
	{
		Awaitable<T> LoadAssetAsync<T>(string address, AsyncToken asyncToken) where T : Object;

		void UnloadAsset<T>(string address) where T : Object;
	}
}