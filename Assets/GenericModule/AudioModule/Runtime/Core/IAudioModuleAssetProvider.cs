using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaAudioModule
{
    public interface IAudioModuleAssetProvider
    {
        bool CheckIsLoaded<T>(string dataId) where T : Object;
        
        UniTask LoadAsset<T>(string dataId) where T : Object;

        T GetAsset<T>(string dataId) where T : Object;

        void UnloadAsset(string dataId);
    }
}