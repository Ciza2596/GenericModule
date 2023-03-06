using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AddressablesModule
{
    public static class AddressablesModuleExtension
    {
        private const string METHOD_NAME = "LoadAssetAsync";

        //version 1
        // public static async UniTask LoadAssetsAsync(this AddressablesModule addressablesModule,
        //     AddressMap[] addressObjectTypeMaps)
        // {
        //     var uniTasks = new List<UniTask>();
        //     foreach (var addressObjectTypeMap in addressObjectTypeMaps)
        //     {
        //         var address = addressObjectTypeMap.Address;
        //         var assetType = addressObjectTypeMap.AssetType;
        //         
        //         var type = GetType(assetType); 
        //         var addressableModuleType = typeof(AddressablesModule);
        //         var methodInfo = addressableModuleType.GetMethod(METHOD_NAME).MakeGenericMethod(type);
        //
        //         uniTasks.Add((UniTask)methodInfo.Invoke(addressablesModule, new object[] { address }));
        //     }
        //
        //     await UniTask.WhenAll(uniTasks);
        // }

        //version2
        public static async UniTask LoadAssetsAsync(this AddressablesModule addressablesModule,
            AddressMap[] addressObjectTypeMaps)
        {
            var uniTasks = new List<UniTask>();
            foreach (var addressObjectTypeMap in addressObjectTypeMaps)
            {
                var address = addressObjectTypeMap.Address;
                var assetType = addressObjectTypeMap.AssetType;
        
                AddUniTask(addressablesModule, address, assetType, uniTasks);
            }
        
            await UniTask.WhenAll(uniTasks);
        }

        public static void UnloadAssets(this AddressablesModule addressablesModule,
            AddressMap[] addressMaps)
        {
            foreach (var addressObjectTypeMap in addressMaps)
            {
                var address = addressObjectTypeMap.Address;
                var assetType = addressObjectTypeMap.AssetType;
                var type = GetType(assetType);

                addressablesModule.UnloadAsset(address, type);
            }
        }

        private static Type GetType(AssetTypes assetType)
        {
            switch (assetType)
            {
                case AssetTypes.Object:
                    return typeof(Object);

                case AssetTypes.Sprite:
                    return typeof(Sprite);
            }

            return typeof(Object);
        }

        private static void AddUniTask(AddressablesModule addressablesModule, string address, AssetTypes assetType,
            List<UniTask> uniTasks)
        {
            switch (assetType)
            {
                case AssetTypes.Object:
                    uniTasks.Add(addressablesModule.LoadAssetAsync<Object>(address));
                    break;

                case AssetTypes.Sprite:
                    uniTasks.Add(addressablesModule.LoadAssetAsync<Sprite>(address));
                    break;
            }
        }
    }
}