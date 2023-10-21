using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaAudioModule.Implement
{
	[CreateAssetMenu(fileName = "AudioModuleAssetProvider", menuName = "Ciza/AudioModule/AssetProvider", order = 1)]
	public class AssetProvider : ScriptableObject, IAssetProvider
	{
		[SerializeField]
		private ClipMapInfo[] _clipMapInfos;

		[SerializeField]
		private PrefabMapInfo[] _prefabMapInfos;

		public UniTask<T> LoadAssetAsync<T>(string dataId, CancellationToken cancellationToken) where T : Object
		{
			var type = typeof(T);
			if (type == typeof(AudioClip))
			{
				var clip = _clipMapInfos.FirstOrDefault(clipMapData => clipMapData.DataId == dataId)?.AudioClip as T;
				return UniTask.FromResult(clip);
			}

			if (type == typeof(GameObject))
			{
				var prefab = _prefabMapInfos.FirstOrDefault(clipMapData => clipMapData.DataId == dataId)?.AudioPrefab as T;
				return UniTask.FromResult(prefab);
			}

			Debug.LogError($"[AudioModuleExampleAssetProvider::GetAsset] Asset is not found by dataId: {dataId}");
			return UniTask.FromResult<T>(null);
		}

		public void UnloadAsset<T>(string dataId) where T : Object { }

		[Serializable]
		private class ClipMapInfo
		{
			[SerializeField]
			private AudioClip _audioClip;

			public string    DataId    => _audioClip.name;
			public AudioClip AudioClip => _audioClip;
		}

		[Serializable]
		private class PrefabMapInfo
		{
			[SerializeField]
			private Audio _audioPrefab;

			public string     DataId      => _audioPrefab.name;
			public GameObject AudioPrefab => _audioPrefab.GameObject;
		}
	}
}
