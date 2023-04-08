using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaAudioModule
{
    public interface IAudioModuleAssetProvider
    {
        UniTask LoadAssets<T>(string[] dataIds) where T : Object;

        void UnloadAssets(string[] dataIds);

        T GetAsset<T>(string dataId) where T : Object;
    }
}