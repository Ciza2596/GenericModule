using System;

namespace CizaEventModule
{
    internal class SyncEventDelegateContainer : BaseEventDelegateContainer
    {
        public void Invoke<T>(T eventData) =>
            ((Action<T>)EventDelegate)?.Invoke(eventData);
    }
}