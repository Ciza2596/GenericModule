using System.Threading.Tasks;
using UnityEngine;

namespace AddressableModule
{
    public static class AddressableModuleExtension
    {
        public static async Task LoadAssetsAsync(this AddressableModule addressableModule,
                                                 AddressMap[] addressObjectTypeMaps)
        {
            foreach (var addressObjectTypeMap in addressObjectTypeMaps)
            {
                var address = addressObjectTypeMap.Address;
                var objectType = addressObjectTypeMap._assetType;

                switch (objectType)
                {
                    case AssetTypes.Object:
                        await addressableModule.GetAssetAsync<Object>(address);
                        break;

                    case AssetTypes.Sprite:
                        await addressableModule.GetAssetAsync<Sprite>(address);
                        break;
                }
            }
        }


        public static void ReleaseAssets(this AddressableModule addressableModule,
                                         AddressMap[] addressObjectTypeMaps)
        {
            foreach (var addressObjectTypeMap in addressObjectTypeMaps)
            {
                var address = addressObjectTypeMap.Address;
                addressableModule.ReleaseAsset(address);
            }
        }
    }
}