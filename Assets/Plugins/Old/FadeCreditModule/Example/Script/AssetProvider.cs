using System;
using System.Linq;
using System.Threading;
using CizaUniTask;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaFadeCreditModule.Example
{
	public class AssetProvider : MonoBehaviour, IAssetProvider
	{
		[SerializeField]
		private PrefabMap[] _rowPrefabMaps;

		[SerializeField]
		private SpriteMap[] _spriteMaps;

		public async UniTask<T> LoadAssetAsync<T>(string address, CancellationToken cancellationToken) where T : Object
		{
			if (typeof(T).Name == nameof(GameObject))
				return (T)(_rowPrefabMaps.FirstOrDefault(prefabMap => prefabMap.DataId == address)?.Obj as Object ?? null);

			if (typeof(T).Name == nameof(Sprite))
				return (T)(_spriteMaps.FirstOrDefault(prefabMap => prefabMap.DataId == address)?.Obj as Object ?? null);

			return null;
		}

		public void UnloadAsset<T>(string address) where T : Object { }


		[Serializable]
		private class Map<T> where T : Object
		{
			[SerializeField]
			private string _dataId;

			[SerializeField]
			private T _obj;

			public string DataId => _dataId;
			public T Obj => _obj;
		}


		[Serializable]
		private class PrefabMap : Map<GameObject> { }

		[Serializable]
		private class SpriteMap : Map<Sprite> { }
	}
}