using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AddressablesModule
{
    public class AddressablesModule
    {
        //private variable
        private Dictionary<Type, Dictionary<string, Object>> _typeAddressObjectMapMap =
            new Dictionary<Type, Dictionary<string, Object>>();

        private Dictionary<string, SceneInstance> _addressSceneMap =
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

        public async UniTask<T> GetAssetAsync<T>(string address) where T : Object
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(address), $"[AddressablesModule::GetAssetAsync] Address is null.");

            var obj = await GetAssetHandleInfo<T>(address);
            return obj;
        }


        public void ReleaseAsset(string address, Type type)
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(address), $"[AddressablesModule::ReleaseAsset] Address is null.");
            Assert.IsTrue(type != null, $"[AddressablesModule::ReleaseAsset] Type is null.");

            if (!_typeAddressObjectMapMap.TryGetValue(type, out var addressObjectMap))
                return;

            if (!addressObjectMap.TryGetValue(address, out var obj))
                return;

            addressObjectMap.Remove(address);
            Addressables.Release(obj);
        }

        public void ReleaseAssets(string[] addressList, Type type)
        {
            Assert.IsTrue(addressList != null, $"[AddressablesModule::ReleaseAssets] AddressList is null.");
            Assert.IsTrue(type != null, $"[AddressablesModule::ReleaseAsset] Type is null.");

            foreach (var address in addressList)
                ReleaseAsset(address, type);
        }

        public void ReleaseAssets(string[] addressList)
        {
            Assert.IsTrue(addressList != null, $"[AddressablesModule::ReleaseAssets] AddressList is null.");

            var types = _typeAddressObjectMapMap.Keys.ToArray();
            foreach (var type in types)
            {
                foreach (var address in addressList)
                    ReleaseAsset(address, type);
            }

            CallGC();
        }

        public void ReleaseAllAssets()
        {
            var types = _typeAddressObjectMapMap.Keys.ToArray();

            foreach (var type in types)
            {
                var assetHandleInfo = _typeAddressObjectMapMap[type];

                var addressList = assetHandleInfo.Keys.ToArray();
                foreach (var address in addressList)
                    ReleaseAsset(address, type);

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
        private async UniTask<T> GetAssetHandleInfo<T>(string address)
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
                obj = await Addressables.LoadAssetAsync<T>(address);
                addressObjectMap.Add(address, obj);
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