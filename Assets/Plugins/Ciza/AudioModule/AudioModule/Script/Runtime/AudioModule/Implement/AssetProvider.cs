using System;
using System.Linq;
using System.Threading;
using CizaUniTask;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaAudioModule.Implement
{
	[CreateAssetMenu(fileName = "AudioModuleAssetProvider", menuName = "Ciza/AudioModule/AssetProvider", order = 1)]
	public class AssetProvider : ScriptableObject, IAssetProvider
	{
		[SerializeField]
		[OverrideDrawer]
		protected ClipMapInfo[] _clipMapInfos;

		[SerializeField]
		[OverrideDrawer]
		protected PrefabMapInfo[] _prefabMapInfos;

		public virtual UniTask<T> LoadAssetAsync<T>(string dataId, CancellationToken cancellationToken) where T : Object
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

		public virtual void UnloadAsset<T>(string dataId) where T : Object { }

		[Serializable]
		protected class ClipMapInfo
		{
			[SerializeField]
			protected AudioClip _audioClip;

			public virtual string DataId => _audioClip.name;
			public virtual AudioClip AudioClip => _audioClip;
		}

		[Serializable]
		protected class PrefabMapInfo
		{
			[SerializeField]
			protected Audio _audioPrefab;

			public virtual string DataId => _audioPrefab.name;
			public virtual GameObject AudioPrefab => _audioPrefab.GameObject;
		}
	}
}