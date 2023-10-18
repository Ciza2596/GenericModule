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

		public async UniTask<T> LoadAssetAsync<T>(string dataId, CancellationToken cancellationToken) where T : Object
		{
			var type = typeof(T);
			if (type == typeof(AudioClip))
				return _clipMapInfos.FirstOrDefault(clipMapData => clipMapData.DataId == dataId)?.AudioClip as T;


			if (type == typeof(GameObject))
				return _prefabMapInfos.FirstOrDefault(clipMapData => clipMapData.DataId == dataId)?.AudioPrefab as T;

			Debug.LogError($"[AudioModuleExampleAssetProvider::GetAsset] Asset is not found by dataId: {dataId}");
			return null;
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
