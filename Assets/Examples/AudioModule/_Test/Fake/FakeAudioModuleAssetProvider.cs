using System.Collections.Generic;
using System.Linq;
using CizaAudioModule;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FakeAudioModuleAssetProvider : IAudioModuleAssetProvider
{
    public const string CLIP_DATA_ID = "ClipDataId";
    public const string PREFAB_DATA_ID = "PrefabDataId";
    public const float SPATIAL_BLEND = 0.5f;

    public static readonly string[] ASSETS_DATA_IDS = new[] { CLIP_DATA_ID, PREFAB_DATA_ID };


    private AudioClip _audioClip;
    private GameObject _audioPrefab;

    public string[] LoadedAssetsDataIds { get; private set; }

    public string[] UnloadedAssetsDataIds { get; private set; }

    public bool CheckIsLoaded<T>(string dataId) where T : Object => true;

    public async UniTask LoadAsset<T>(string dataId) where T : Object
    {
        if (LoadedAssetsDataIds is null)
            LoadedAssetsDataIds = new []{dataId};

        else
            LoadedAssetsDataIds = LoadedAssetsDataIds.Concat(new []{dataId}).ToArray();
        
        await UniTask.CompletedTask;
    }

    public T GetAsset<T>(string dataId) where T : Object
    {
        if (dataId == CLIP_DATA_ID)
            return _audioClip as T;

        if (dataId == PREFAB_DATA_ID)
            return _audioPrefab as T;

        return null;
    }

    public void UnloadAsset(string dataId)
    {
        if (UnloadedAssetsDataIds is null)
            UnloadedAssetsDataIds = new []{dataId};

        else
            UnloadedAssetsDataIds = UnloadedAssetsDataIds.Concat(new []{dataId}).ToArray();
    }

    public void SetClip(AudioClip audioClip) =>
        _audioClip = audioClip;

    public void SetAudioPrefab(GameObject audioPrefab) =>
        _audioPrefab = audioPrefab;
}