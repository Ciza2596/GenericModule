using System;
using Cysharp.Threading.Tasks;

namespace EventModule
{
    public class AsyncEventDelegateContainer : BaseEventDelegateContainer
    {
        public async UniTask Invoke<T>(T eventData)
        {
            await ((Func<T, UniTask>)EventDelegate).Invoke(eventData);
        }
    }
}