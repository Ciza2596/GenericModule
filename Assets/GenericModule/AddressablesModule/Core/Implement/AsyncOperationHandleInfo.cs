using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AddressablesModule.Componet
{
    public class AsyncOperationHandleInfo<T> : IAsyncOperationHandleInfo where T : Object
    {
        private AsyncOperationHandle<T> _handle;

        public AsyncOperationHandleInfo(AsyncOperationHandle<T> handle) =>
            _handle = handle;

        async Task IAsyncOperationHandleInfo.Task() =>
            await _handle.Task.ConfigureAwait(false);
        
        Object IAsyncOperationHandleInfo.Result => _handle.Result;
    }
}