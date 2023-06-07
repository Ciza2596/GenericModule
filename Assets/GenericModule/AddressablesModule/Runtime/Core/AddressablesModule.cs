using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CizaAddressablesModule
{
    public class AddressablesModule
    {
        //private variable
        private readonly Dictionary<Type, Dictionary<string, Object>> _typeAddressObjectMapMap =
            new Dictionary<Type, Dictionary<string, Object>>();

        private readonly Dictionary<string, SceneInstance> _addressSceneMap =
            new Dictionary<string,SceneInstance>();

        //public method

        //asset
        public T GetAsset<T>(string address) where T : Object
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(address), $"[AddressablesModule::GetAsset] Address is null.");

            var type = typeof(T);
            var hasAddressObjectMap = _typeAddressObjectMapMap.TryGetValue(type, out var addressObjectMap);
            Assert.IsTrue(hasAddressObjectMap,
                $"[AddressablesModule::GetAsset] Type: {type} doesnt exist in typeAssetHandleInfos.");

            var hasObj = addressObjectMap.TryGetValue(address, out var obj);
            Assert.IsTrue(hasObj,
                $"[AddressablesModule::GetAsset] Address: {address} doesnt exist in assetHandleInfos.");

            return obj as T;
        }

        public async UniTask<T> LoadAssetAsync<T>(string address, CancellationToken cancellationToken = default) where T : Object
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(address), $"[AddressablesModule::LoadAssetAsync] Address is null.");
            
            var obj = await GetAssetHandleInfo<T>(address, cancellationToken);
            return obj;
        }

        public void UnloadAsset<T>(string address) where T: Object
        {
            var type = typeof(T);
            UnloadAsset(address, type);
        }

        public void UnloadAsset(string address, Type type)
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(address), $"[AddressablesModule::UnloadAsset] Address is null.");
            Assert.IsTrue(type != null, $"[AddressablesModule::UnloadAsset] Type is null.");

            if (!_typeAddressObjectMapMap.TryGetValue(type, out var addressObjectMap))
                return;

            if (!addressObjectMap.TryGetValue(address, out var obj))
                return;

            addressObjectMap.Remove(address);
            Addressables.Release(obj);
        }

        public void UnloadAssets(string[] addressList, Type type)
        {
            Assert.IsTrue(addressList != null, $"[AddressablesModule::UnloadAssets] AddressList is null.");
            Assert.IsTrue(type != null, $"[AddressablesModule::UnloadAssets] Type is null.");

            foreach (var address in addressList)
                UnloadAsset(address, type);
        }

        public void UnloadAssets(string[] addressList)
        {
            Assert.IsTrue(addressList != null, $"[AddressablesModule::UnloadAssets] AddressList is null.");

            var types = _typeAddressObjectMapMap.Keys.ToArray();
            foreach (var type in types)
            {
                foreach (var address in addressList)
                    UnloadAsset(address, type);
            }

            CallGC();
        }

        public void UnloadAllAssets(Type type)
        {
            if (!_typeAddressObjectMapMap.ContainsKey(type))
            {
                Debug.LogWarning($"[AddressablesModule::UnloadAllAssets] Not find {type} is loaded");
                return;
            }

            var assetHandleInfo = _typeAddressObjectMapMap[type];
            var addressList = assetHandleInfo.Keys.ToArray();
            foreach (var address in addressList)
                UnloadAsset(address, type);

            assetHandleInfo.Clear();

            CallGC();
        }
        
        public void UnloadAllAssets()
        {
            var types = _typeAddressObjectMapMap.Keys.ToArray();

            foreach (var type in types)
            {
                var assetHandleInfo = _typeAddressObjectMapMap[type];

                var addressList = assetHandleInfo.Keys.ToArray();
                foreach (var address in addressList)
                    UnloadAsset(address, type);

                assetHandleInfo.Clear();
            }

            _typeAddressObjectMapMap.Clear();

            CallGC();
        }


        //scene
        public void ActivateScene(string address)
        {
            Assert.IsTrue(_addressSceneMap.ContainsKey(address),
                $"[AddressablesModule::ActivateScene] Address: {address} not find info.");
            var scene = _addressSceneMap[address];
            scene.ActivateAsync();
        }

        public async UniTask<SceneInstance> LoadSceneAsync(string address, LoadSceneMode loadMode = LoadSceneMode.Single,
            bool isActivateOnLoad = true)
        {
            var scene = await Addressables.LoadSceneAsync(address, loadMode, false);
            _addressSceneMap.Add(address, scene);

            if (isActivateOnLoad)
                ActivateScene(address);

            return scene;
        }

        public async UniTask UnloadSceneAsync(string address)
        {
            Assert.IsTrue(_addressSceneMap.ContainsKey(address),
                "[AddressablesModule::UnloadSceneAsync] Cant unload scene. Not find sceneHandle");

            var scene = _addressSceneMap[address];
            _addressSceneMap.Remove(address);

            await Addressables.UnloadSceneAsync(scene);
        }


        //private method
        private async UniTask<T> GetAssetHandleInfo<T>(string address, CancellationToken cancellationToken)
            where T : Object
        {
            var type = typeof(T);
            if (type.IsSubclassOf(typeof(Component)))
            {
                Debug.LogError(
                    $"[AddressablesModule::GetAssetHandleInfo] Get asset form ass just support Object type, not support component.");
                return null;
            }

            if (!_typeAddressObjectMapMap.TryGetValue(type, out var addressObjectMap))
            {
                addressObjectMap = new Dictionary<string, Object>();
                _typeAddressObjectMapMap.Add(type, addressObjectMap);
            }

            if (!addressObjectMap.TryGetValue(address, out var obj))
            {
                try
                {
                    obj = await Addressables.LoadAssetAsync<T>(address).WithCancellation(cancellationToken);
                    addressObjectMap.Add(address, obj);
                }
                catch
                {
                    Debug.Log($"[AddressablesModule::GetAssetHandleInfo] Loading assets with address: {address} is canceled.");
                    return null;
                }
            }

            return obj as T;
        }

        private void CallGC()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}