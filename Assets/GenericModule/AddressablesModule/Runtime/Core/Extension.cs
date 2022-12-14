using System;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AddressablesModule
{
    public static class Extension
    {
        private const string METHOD_NAME = "GetAssetAsync";

        
        
        public static async Task LoadAssetsAsync(this AddressablesModule addressablesModule,
            AddressMap[] addressObjectTypeMaps)
        {
            foreach (var addressObjectTypeMap in addressObjectTypeMaps)
            {
                var address = addressObjectTypeMap.Address;
                var assetType = addressObjectTypeMap.AssetType;
                
                var type = GetType(assetType);
                var addressableModuleType = typeof(AddressablesModule);
                var methodInfo = addressableModuleType.GetMethod(METHOD_NAME).MakeGenericMethod(type);

                await (Task)methodInfo.Invoke(addressablesModule, new object[] { address });
            }
        }

        public static void ReleaseAssets(this AddressablesModule addressablesModule,
            AddressMap[] addressMaps)
        {
            foreach (var addressObjectTypeMap in addressMaps)
            {
                var address = addressObjectTypeMap.Address;
                var assetType = addressObjectTypeMap.AssetType;
                var type = GetType(assetType);

                addressablesModule.ReleaseAsset(address, type);
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
    }
}