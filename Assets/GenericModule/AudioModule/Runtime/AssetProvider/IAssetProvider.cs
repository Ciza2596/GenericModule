using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaAudioModule
{
	public interface IAssetProvider
	{
		UniTask<T> LoadAssetAsync<T>(string dataId, CancellationToken cancellationToken) where T : Object;

		void UnloadAsset<T>(string dataId) where T : Object;
	}
}
