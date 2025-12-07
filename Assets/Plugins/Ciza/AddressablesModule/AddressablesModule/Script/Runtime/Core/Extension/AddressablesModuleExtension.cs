using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaAddressablesModule
{
	public static class AddressablesModuleExtension
	{
		// public static async UniTask LoadAssetsAsync(this AddressablesModule addressablesModule, AddressMap[] addressObjectTypeMaps)
		// {
		// 	var uniTasks = new List<UniTask>();
		// 	foreach (var addressObjectTypeMap in addressObjectTypeMaps)
		// 	{
		// 		var address   = addressObjectTypeMap.Address;
		// 		var assetType = addressObjectTypeMap.AssetType;
		//
		// 		AddUniTask(addressablesModule, address, assetType, uniTasks);
		// 	}
		//
		// 	await UniTask.WhenAll(uniTasks);
		// }
		//
		// public static void UnloadAssets(this AddressablesModule addressablesModule, AddressMap[] addressMaps)
		// {
		// 	foreach (var addressObjectTypeMap in addressMaps)
		// 	{
		// 		var address   = addressObjectTypeMap.Address;
		// 		var assetType = addressObjectTypeMap.AssetType;
		// 		var type      = GetType(assetType);
		//
		// 		addressablesModule.UnloadAsset(address, type);
		// 	}
		// }
		//
		// private static Type GetType(AssetTypes assetType)
		// {
		// 	switch (assetType)
		// 	{
		// 		case AssetTypes.GameObject:
		// 			return typeof(Object);
		//
		// 		case AssetTypes.Sprite:
		// 			return typeof(Sprite);
		// 	}
		//
		// 	return typeof(Object);
		// }
		//
		// private static void AddUniTask(AddressablesModule addressablesModule, string address, AssetTypes assetType, List<UniTask> uniTasks)
		// {
		// 	switch (assetType)
		// 	{
		// 		case AssetTypes.GameObject:
		// 			uniTasks.Add(addressablesModule.LoadAssetAsync<Object>(address));
		// 			break;
		//
		// 		case AssetTypes.Sprite:
		// 			uniTasks.Add(addressablesModule.LoadAssetAsync<Sprite>(address));
		// 			break;
		// 	}
		// }
	}
}