using System;
using System.Collections.Generic;
using CizaAsync;
using UnityEngine;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace CizaAddressablesModule
{
	public class AddressablesByRefCountModule
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected readonly AddressablesModule _addressablesModule;
		protected readonly Dictionary<string, int> _refCountMapByAddress = new Dictionary<string, int>();

		// EVENT: ---------------------------------------------------------------------------------

		public event Func<string, AsyncToken, Awaitable> OnLoadAssetAsync;
		public event Action<string> OnUnloadAsset;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual IReadOnlyDictionary<string, int> RefCountMapByAddress => _refCountMapByAddress;

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public AddressablesByRefCountModule(string className) =>
			_addressablesModule = new AddressablesModule(className);

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual async Awaitable<T> LoadAssetAsync<T>(string address, AsyncToken asyncToken) where T : Object
		{
			await _addressablesModule.LoadAssetAsync<T>(address, asyncToken);
			AddRefCount(address);
			if (OnLoadAssetAsync != null)
				await OnLoadAssetAsync.Invoke(address, asyncToken);
			return GetAsset<T>(address);
		}

		public virtual T GetAsset<T>(string address) where T : Object
		{
			if (!_refCountMapByAddress.ContainsKey(address))
				return null;

			return _addressablesModule.GetAsset<T>(address);
		}

		public virtual void UnloadAsset<T>(string address) where T : Object
		{
			if (!_refCountMapByAddress.ContainsKey(address))
				return;

			_refCountMapByAddress[address] -= 1;
			OnUnloadAsset?.Invoke(address);

			if (_refCountMapByAddress[address] <= 0)
			{
				_refCountMapByAddress.Remove(address);
				_addressablesModule.UnloadAsset<T>(address);
			}
		}

		public virtual void UnloadAllAssets()
		{
			_addressablesModule.UnloadAllAssets();
			_refCountMapByAddress.Clear();
		}

		// PROTECT METHOD: --------------------------------------------------------------------

		protected virtual void AddRefCount(string address, int count = 1)
		{
			_refCountMapByAddress.TryAdd(address, 0);
			_refCountMapByAddress[address] += count;
		}
	}
}