using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaAudioModule.Implement
{
    [CreateAssetMenu(fileName = "AudioModuleAssetProvider", menuName = "Ciza/AudioModule/AudioModuleAssetProvider", order = 1)]
    public class AudioModuleAssetProvider : ScriptableObject, IAudioModuleAssetProvider
    {
        [SerializeField] private ClipMapData[] _clipMapDatas;
        [SerializeField] private PrefabMapData[] _prefabMapDatas;

        public bool CheckIsLoad<T>(string dataId) where T : Object =>
            true;

        public UniTask LoadAsset<T>(string dataId) where T : Object =>
            UniTask.CompletedTask;

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

        public void UnloadAsset(string dataId) { }
        

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