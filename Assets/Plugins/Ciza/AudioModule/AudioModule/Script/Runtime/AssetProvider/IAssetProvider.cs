using CizaAsync;
using UnityEngine;

namespace CizaAudioModule
{
	public interface IAssetProvider
	{
		Awaitable<T> LoadAssetAsync<T>(string dataId, AsyncToken asyncToken) where T : Object;

		void UnloadAsset<T>(string dataId) where T : Object;
	}
}