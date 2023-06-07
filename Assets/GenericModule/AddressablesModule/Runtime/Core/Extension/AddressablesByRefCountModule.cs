using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaAddressablesModule
{
	public class AddressablesByRefCountModule
	{
		private readonly AddressablesModule      _addressablesModule   = new AddressablesModule();
		private readonly Dictionary<string, int> _refCountMapByAddress = new Dictionary<string, int>();

		public IReadOnlyDictionary<string, int> RefCountMapByAddress => _refCountMapByAddress;

		public async UniTask<T> LoadAssetAsync<T>(string address, CancellationToken cancellationToken = default) where T : Object
		{
			var asset = await _addressablesModule.LoadAssetAsync<T>(address, cancellationToken);
			AddRefCount(address);
			return asset;
		}

		public T GetAsset<T>(string address) where T : Object =>
			_addressablesModule.GetAsset<T>(address);

		public void UnloadAsset<T>(string address) where T : Object
		{
			_addressablesModule.UnloadAsset<T>(address);
			RemoveRefCount(address);
		}

		public void UnloadAllAssets()
		{
			_addressablesModule.UnloadAllAssets();
			_refCountMapByAddress.Clear();
		}

		private void AddRefCount(string address, int count = 1)
		{
			if (!_refCountMapByAddress.ContainsKey(address))
				_refCountMapByAddress.Add(address, 0);

			_refCountMapByAddress[address] += count;
		}

		private void RemoveRefCount(string address, int count = 1)
		{
			if (!_refCountMapByAddress.ContainsKey(address))
				return;

			_refCountMapByAddress[address] -= count;

			if (_refCountMapByAddress[address] <= 0)
				_refCountMapByAddress.Remove(address);
		}
	}
}
