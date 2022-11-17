using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AddressableModule.Componet;
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
        private Dictionary<Type, Dictionary<string, IAsyncOperationHandleInfo>> _typeAssetHandleInfos =
            new Dictionary<Type, Dictionary<string, IAsyncOperationHandleInfo>>();

        private Dictionary<string, AsyncOperationHandle<SceneInstance>> _sceneHandles =
            new Dictionary<string, AsyncOperationHandle<SceneInstance>>();


        //public method

        //asset
        public T GetAsset<T>(string address) where T : Object
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(address), $"[AddressableModule::GetAsset] Address is null.");
            
            var type = typeof(T);
            var hasTypeAssetHandleInfo = _typeAssetHandleInfos.TryGetValue(type, out var assetHandleInfos);
            Assert.IsTrue(hasTypeAssetHandleInfo,$"[AddressableModule::GetAsset] Type: {type} doesnt exist in typeAssetHandleInfos.");

            var hasAssetHandleInfo = assetHandleInfos.TryGetValue(address, out var assetHandleInfo);
            Assert.IsTrue(hasAssetHandleInfo,$"[AddressableModule::GetAsset] Address: {address} doesnt exist in assetHandleInfos.");

            
            var result = assetHandleInfo.Result;
            return result as T;
        }

        public async Task<T> GetAssetAsync<T>(string address) where T : Object
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(address), $"[AddressableModule::GetAssetAsync] Address is null.");

            var assetHandleInfo = await GetAssetHandleInfo<T>(address);
            var result = assetHandleInfo.Result;
            
            return result as T;
        }
        
        

        public void ReleaseAsset(string address, Type type)
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(address), $"[AddressableModule::ReleaseAsset] Address is null.");
            Assert.IsTrue(type != null, $"[AddressableModule::ReleaseAsset] Type is null.");

            var hasTypeAssetHandleInfo = _typeAssetHandleInfos.TryGetValue(type, out var assetHandleInfos);
            if (!hasTypeAssetHandleInfo)
                return;

            var hasAssetHandleInfo = assetHandleInfos.TryGetValue(address, out var assetHandleInfo);
            if(!hasAssetHandleInfo)
                return;
            
            assetHandleInfos.Remove(address);

            var result = assetHandleInfo.Result;
            Addressables.Release(result);
        }
        
        public void ReleaseAssets(string[] addressList, Type type)
        {
            Assert.IsTrue(addressList != null, $"[AddressableModule::ReleaseAssets] AddressList is null.");
            Assert.IsTrue(type != null, $"[AddressableModule::ReleaseAsset] Type is null.");
            
            foreach (var address in addressList)
                ReleaseAsset(address, type);
        }

        public void ReleaseAssets(string[] addressList)
        {
            Assert.IsTrue(addressList != null, $"[AddressableModule::ReleaseAssets] AddressList is null.");
            
            var types = _typeAssetHandleInfos.Keys.ToArray();
            foreach (var type in types)
            {
                foreach (var address in addressList)
                    ReleaseAsset(address, type);
            }

            CallGC();
        }

        public void ReleaseAllAssets()
        {
            var types = _typeAssetHandleInfos.Keys.ToArray();

            foreach (var type in types)
            {
                var assetHandleInfo = _typeAssetHandleInfos[type];
                
                var addressList = assetHandleInfo.Keys.ToArray();
                foreach (var address in addressList)
                    ReleaseAsset(address, type);
                
                assetHandleInfo.Clear();
            }

            _typeAssetHandleInfos.Clear();
            
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

        
        //private method
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

            
            if (!_typeAssetHandleInfos.TryGetValue(type, out var assetHandleInfos))
            {
                assetHandleInfos = new Dictionary<string, IAsyncOperationHandleInfo>();
                _typeAssetHandleInfos.Add(type, assetHandleInfos);
            }

            var hasAssetHandleInfo = assetHandleInfos.TryGetValue(address, out var assetHandleInfo);

            if (hasAssetHandleInfo)
                await assetHandleInfo.Task();

            if (assetHandleInfo != null && assetHandleInfo.Result == null)
            {
                ReleaseAsset(address, type);
                hasAssetHandleInfo = false;
            }

            if (!hasAssetHandleInfo)
            {
                var handle = Addressables.LoadAssetAsync<T>(address);
                assetHandleInfo = new AsyncOperationHandleInfo<T>(handle);
                await assetHandleInfo.Task();
                
                assetHandleInfos.Add(address, assetHandleInfo);
            }

            return assetHandleInfo;
        }
        
        private void CallGC()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}