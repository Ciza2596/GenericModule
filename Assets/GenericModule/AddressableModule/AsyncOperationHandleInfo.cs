using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AddressableModule
{
    public class AsyncOperationHandleInfo<T> : IAsyncOperationHandleInfo where T : Object
    {
        private AsyncOperationHandle<T> _handle;

        public AsyncOperationHandleInfo(AsyncOperationHandle<T> handle) =>
            _handle = handle;

        bool IAsyncOperationHandleInfo.IsDone => _handle.IsDone;

        async Task IAsyncOperationHandleInfo.Task() =>
            await _handle.Task.ConfigureAwait(false);
        
        Object IAsyncOperationHandleInfo.Result => _handle.Result;
    }
}