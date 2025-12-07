using System;
using System.Collections.Generic;
using System.Threading;
using CizaUniTask;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace CizaAddressablesModule
{
    public class AddressablesByRefCountModule
    {
        private readonly AddressablesModule _addressablesModule;
        private readonly Dictionary<string, int> _refCountMapByAddress = new Dictionary<string, int>();

        public event Func<string, UniTask> OnLoadAssetAsync;
        public event Action<string> OnUnloadAsset;
        
        public IReadOnlyDictionary<string, int> RefCountMapByAddress => _refCountMapByAddress;

        [Preserve]
        public AddressablesByRefCountModule(string className) =>
            _addressablesModule = new AddressablesModule(className);

        public async UniTask<T> LoadAssetAsync<T>(string address, CancellationToken cancellationToken = default) where T : Object
        {
            await _addressablesModule.LoadAssetAsync<T>(address, cancellationToken);
            AddRefCount(address);
            if (OnLoadAssetAsync != null)
                await OnLoadAssetAsync.Invoke(address);
            return GetAsset<T>(address);
        }

        public T GetAsset<T>(string address) where T : Object
        {
            if (!_refCountMapByAddress.ContainsKey(address))
                return null;

            return _addressablesModule.GetAsset<T>(address);
        }

        public void UnloadAsset<T>(string address) where T : Object
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

        public void UnloadAllAssets()
        {
            _addressablesModule.UnloadAllAssets();
            _refCountMapByAddress.Clear();
        }

        private void AddRefCount(string address, int count = 1)
        {
            _refCountMapByAddress.TryAdd(address, 0);
            _refCountMapByAddress[address] += count;
        }
    }
}