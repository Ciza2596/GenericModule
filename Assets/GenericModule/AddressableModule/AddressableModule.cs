using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AddressableModule
{
    public class AddressableModule
    {
        //private variable
        private Dictionary<string, IAsyncOperationHandleInfo> _assetHandleInfos =
            new Dictionary<string, IAsyncOperationHandleInfo>();

        private Dictionary<string, AsyncOperationHandle<SceneInstance>> _sceneHandles =
            new Dictionary<string, AsyncOperationHandle<SceneInstance>>();


        //public method

        //asset
        public async Task LoadAssetsAsync(string[] addressList)
        {
            Assert.IsFalse(addressList == null, $"[AddressableModule::LoadAssetsAsync] AddressList is null.");

            foreach (var address in addressList)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(address), $"[AddressableModule::LoadAssetsAsync] Address is null.");
                await GetAssetHandleInfo(address);
            }
        }

        public T GetAsset<T>(string address) where T : Object
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(address), $"[AddressableModule::GetAsset] Address is null.");
            
            var handleInfo = _assetHandleInfos[address];
            Assert.IsTrue(handleInfo.IsDone,$"[AddressableModule::GetAsset] Address: {address} is not done.");

            var result = handleInfo.Result;
            return result as T;
        }

        public async Task<T> GetAssetAsync<T>(string address) where T : Object
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(address), $"[AddressableModule::GetAssetAsync] Address is null.");

            var handleInfo = await GetAssetHandleInfo(address);
            var result = handleInfo.Result;
            
            return result as T;
        }

        public void ReleaseAsset(string address)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(address), $"[AddressableModule::ReleaseAsset] Address is null.");

            var hasValue = _assetHandleInfos.TryGetValue(address, out var info);

            if (!hasValue)
                return;

            _assetHandleInfos.Remove(address);
            Addressables.Release(info.Result);
        }

        public void ReleaseAssets(string[] addressList)
        {
            Assert.IsFalse(addressList == null, $"[AddressableModule::ReleaseAssets] AddressList is null.");

            foreach (var address in addressList)
                ReleaseAsset(address);

            CallGC();
        }

        public void ReleaseAllAsset()
        {
            var addressList = _assetHandleInfos.Keys;
            foreach (var address in addressList)
                ReleaseAsset(address);

            _assetHandleInfos.Clear();
            
            CallGC();
        }


        //scene
        public async Task LoadSceneAsync(string address, LoadSceneMode loadMode = LoadSceneMode.Additive,
            bool activateOnLoad = true, int priority = 100)
        {
            var asyncOperationHandle = Addressables.LoadSceneAsync(address, loadMode, activateOnLoad, priority);
            await asyncOperationHandle.Task;

            SceneManager.SetActiveScene(asyncOperationHandle.Result.Scene);

            _sceneHandles.Add(address, asyncOperationHandle);
        }

        public async Task UnloadSceneAsync(string address)
        {
            if (!_sceneHandles.ContainsKey(address))
                return;

            var asyncSceneHandle = _sceneHandles[address];
            _sceneHandles.Remove(address);

            await Addressables.UnloadSceneAsync(asyncSceneHandle, true).Task;
        }


        private async Task<IAsyncOperationHandleInfo> GetAssetHandleInfo(string address)
        {
            var handleInfo = await GetAssetHandleInfo<Object>(address);
            return handleInfo;
        }

        private async Task<IAsyncOperationHandleInfo> GetAssetHandleInfo<T>(string address)
            where T : Object
        {
            var type = typeof(T);
            if (type.IsSubclassOf(typeof(Component)))
            {
                Debug.LogError(
                    $"[AddressableModule::GetAssetHandleInfo] Get asset form ass just support Object type, not support component.");
                return null;
            }

            var hasValue = _assetHandleInfos.TryGetValue(address, out var handleInfo);

            if (hasValue)
                await handleInfo.Task();

            if (handleInfo != null && handleInfo.Result == null)
            {
                ReleaseAsset(address);

                var handle = Addressables.LoadAssetAsync<T>(address);
                handleInfo = new AsyncOperationHandleInfo<T>(handle);
                _assetHandleInfos.Add(address, handleInfo);
                await handleInfo.Task();
            }

            return handleInfo;
        }

        private void CallGC()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}