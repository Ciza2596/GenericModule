using System;

namespace EventModule
{
    public class SyncEventDelegateContainer : BaseEventDelegateContainer
    {
        public void Invoke<T>(T eventData) =>
            ((Action<T>)EventDelegate)?.Invoke(eventData);
    }
}