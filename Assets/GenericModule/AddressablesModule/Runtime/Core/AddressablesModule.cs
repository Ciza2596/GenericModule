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
		private readonly string _className;

		private readonly Dictionary<Type, Dictionary<string, Object>> _objectMapByAddressMapByType = new Dictionary<Type, Dictionary<string, Object>>();
		private readonly Dictionary<string, SceneInstance>            _addressSceneMap             = new Dictionary<string, SceneInstance>();

		public AddressablesModule() : this("AddressablesModule") { }

		public AddressablesModule(string className) =>
			_className = className;

		//public method

		//asset
		public T GetAsset<T>(string address) where T : Object
		{
			Assert.IsTrue(!string.IsNullOrWhiteSpace(address), $"[{_className}::GetAsset] Address is null.");

			var type                = typeof(T);
			var hasAddressObjectMap = _objectMapByAddressMapByType.TryGetValue(type, out var addressObjectMap);
			Assert.IsTrue(hasAddressObjectMap, $"[{_className}::GetAsset] Type: {type} doesnt exist in typeAssetHandleInfos.");

			var hasObj = addressObjectMap.TryGetValue(address, out var obj);
			Assert.IsTrue(hasObj, $"[{_className}::GetAsset] Address: {address} doesnt exist in assetHandleInfos.");

			return obj as T;
		}

		public async UniTask<T> LoadAssetAsync<T>(string address, CancellationToken cancellationToken = default) where T : Object
		{
			Assert.IsTrue(!string.IsNullOrWhiteSpace(address), $"[{_className}::LoadAssetAsync] Address is null.");

			var obj = await GetAssetHandleInfo<T>(address, cancellationToken);
			return obj;
		}

		public void UnloadAsset<T>(string address) where T : Object
		{
			var type = typeof(T);
			UnloadAsset(address, type);
		}

		public void UnloadAsset(string address, Type type)
		{
			Assert.IsTrue(!string.IsNullOrWhiteSpace(address), $"[{_className}::UnloadAsset] Address is null.");
			Assert.IsTrue(type != null, $"[{_className}::UnloadAsset] Type is null.");

			if (!_objectMapByAddressMapByType.TryGetValue(type, out var addressObjectMap))
				return;

			if (!addressObjectMap.TryGetValue(address, out var obj))
				return;

			addressObjectMap.Remove(address);
			try
			{
				Addressables.Release(obj);
			}
			catch (Exception e)
			{
				// ignored
			}
		}

		public void UnloadAssets(string[] addressList, Type type)
		{
			Assert.IsTrue(addressList != null, $"[{_className}::UnloadAssets] AddressList is null.");
			Assert.IsTrue(type        != null, $"[{_className}::UnloadAssets] Type is null.");

			foreach (var address in addressList)
				UnloadAsset(address, type);
		}

		public void UnloadAssets(string[] addressList)
		{
			Assert.IsTrue(addressList != null, $"[{_className}::UnloadAssets] AddressList is null.");

			var types = _objectMapByAddressMapByType.Keys.ToArray();
			foreach (var type in types)
			{
				foreach (var address in addressList)
					UnloadAsset(address, type);
			}

			CallGC();
		}

		public void UnloadAllAssets(Type type)
		{
			if (!_objectMapByAddressMapByType.ContainsKey(type))
			{
				Debug.LogWarning($"[{_className}::UnloadAllAssets] Not find {type} is loaded");
				return;
			}

			var assetHandleInfo = _objectMapByAddressMapByType[type];
			var addressList     = assetHandleInfo.Keys.ToArray();
			foreach (var address in addressList)
				UnloadAsset(address, type);

			assetHandleInfo.Clear();

			CallGC();
		}

		public void UnloadAllAssets()
		{
			var types = _objectMapByAddressMapByType.Keys.ToArray();

			foreach (var type in types)
			{
				var assetHandleInfo = _objectMapByAddressMapByType[type];

				var addressList = assetHandleInfo.Keys.ToArray();
				foreach (var address in addressList)
					UnloadAsset(address, type);

				assetHandleInfo.Clear();
			}

			_objectMapByAddressMapByType.Clear();

			CallGC();
		}

		//scene
		public void ActivateScene(string address)
		{
			Assert.IsTrue(_addressSceneMap.ContainsKey(address), $"[{_className}::ActivateScene] Address: {address} not find info.");
			var scene = _addressSceneMap[address];
			scene.ActivateAsync();
		}

		public async UniTask<SceneInstance> LoadSceneAsync(string address, LoadSceneMode loadMode = LoadSceneMode.Single, bool isActivateOnLoad = true)
		{
			var scene = await Addressables.LoadSceneAsync(address, loadMode, false);
			_addressSceneMap.Add(address, scene);

			if (isActivateOnLoad)
				ActivateScene(address);

			return scene;
		}

		public async UniTask UnloadSceneAsync(string address)
		{
			Assert.IsTrue(_addressSceneMap.ContainsKey(address), $"[{_className}::UnloadSceneAsync] Cant unload scene. Not find sceneHandle");

			var scene = _addressSceneMap[address];
			_addressSceneMap.Remove(address);

			await Addressables.UnloadSceneAsync(scene);
		}

		//private method
		private async UniTask<T> GetAssetHandleInfo<T>(string address, CancellationToken cancellationToken) where T : Object
		{
			var type = typeof(T);
			if (type.IsSubclassOf(typeof(Component)))
			{
				Debug.LogError($"[{_className}::GetAssetHandleInfo] Get asset form ass just support Object type, not support component.");
				return null;
			}

			if (!_objectMapByAddressMapByType.TryGetValue(type, out var addressObjectMap))
			{
				addressObjectMap = new Dictionary<string, Object>();
				_objectMapByAddressMapByType.Add(type, addressObjectMap);
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
