using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace EventModule
{
    internal class AsyncEventDelegateContainer : BaseEventDelegateContainer
    {
        public async UniTask Invoke<T>(T eventData)
        {
            var invocationTasks = new List<UniTask>();

            foreach (var invocation in EventDelegate.GetInvocationList())
                invocationTasks.Add(((Func<T, UniTask>)invocation).Invoke(eventData));

            await UniTask.WhenAll(invocationTasks);
        }
    }
}