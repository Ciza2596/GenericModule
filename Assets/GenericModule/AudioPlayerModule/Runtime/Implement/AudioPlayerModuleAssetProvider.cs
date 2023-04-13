using System;
using System.Linq;
using CizaAudioModule;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaAudioPlayerModule.Implement
{
    [CreateAssetMenu(fileName = "AudioPlayerModuleAssetProvider", menuName = "Ciza/AudioPlayerModule/AudioPlayerModuleAssetProvider", order = 1)]
    public class AudioPlayerModuleAssetProvider : ScriptableObject, IAudioModuleAssetProvider
    {
        [SerializeField] private ClipMapData[] _clipMapDatas;
        [SerializeField] private PrefabMapData[] _prefabMapDatas;

        public UniTask LoadAssets<T>(string[] dataIds) where T : Object =>
            UniTask.CompletedTask;

        public void UnloadAssets(string[] dataIds)
        {
        }

        public T GetAsset<T>(string dataId) where T : Object
        {
            var type = typeof(T);
            if (type == typeof(AudioClip))
                return _clipMapDatas.Where(clipMapData => clipMapData.DataId == dataId).FirstOrDefault()?.AudioClip as T;


            if (type == typeof(GameObject))
                return _prefabMapDatas.Where(clipMapData => clipMapData.DataId == dataId).FirstOrDefault()?.AudioPrefab as T;

            Debug.LogError($"[AudioModuleExampleAssetProvider::GetAsset] Asset is not found by dataId: {dataId}");
            return null;
        }


        [Serializable]
        private class ClipMapData
        {
            [SerializeField] private AudioClip _audioClip;

            public string DataId => _audioClip.name;
            public AudioClip AudioClip => _audioClip;
        }

        [Serializable]
        private class PrefabMapData
        {
            [SerializeField] private Audio _audioPrefab;

            public string DataId => _audioPrefab.name;
            public GameObject AudioPrefab => _audioPrefab.GameObject;
        }
    }
}