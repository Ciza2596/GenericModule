using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace EventModule
{
    public abstract class EventModule<TBaseEvent, TSyncEvent, TAsyncEvent>
        where TSyncEvent : TBaseEvent where TAsyncEvent : TBaseEvent
    {
        //private method
        private readonly Dictionary<Type, SyncEventDelegateContainer> _syncEvents =
            new Dictionary<Type, SyncEventDelegateContainer>();

        private readonly Dictionary<Type, AsyncEventDelegateContainer> _asyncEvents =
            new Dictionary<Type, AsyncEventDelegateContainer>();


        //public variable
        public int SyncEventCount => _syncEvents.Count;
        public int AsyncEventCount => _asyncEvents.Count;


        //public method

        //sync
        public void AddSyncListener<TEvent>(Action<TEvent> eventDelegate) where TEvent : TSyncEvent =>
            AddListener<TEvent, SyncEventDelegateContainer>(eventDelegate, _syncEvents);

        public void RemoveSyncListener<TEvent>(Action<TEvent> eventDelegate) where TEvent : TSyncEvent =>
            RemoveListener<TEvent, SyncEventDelegateContainer>(eventDelegate, _syncEvents);


        public void NotifySyncEvent<TEvent>(TEvent eventData) where TEvent : TSyncEvent
        {
            if (_syncEvents.TryGetValue(typeof(TEvent), out var syncEventDelegateContainer))
                syncEventDelegateContainer?.Invoke(eventData);
        }


        //async
        public void AddAsyncListener<TEvent>(Func<TEvent, CancellationToken, UniTask> eventDelegate)
            where TEvent : TAsyncEvent =>
            AddListener<TEvent, AsyncEventDelegateContainer>(eventDelegate, _asyncEvents);

        public void RemoveAsyncListener<TEvent>(Func<TEvent, CancellationToken, UniTask> eventDelegate)
            where TEvent : TAsyncEvent =>
            RemoveListener<TEvent, AsyncEventDelegateContainer>(eventDelegate, _asyncEvents);


        public async UniTask NotifyAsyncEvent<TEvent>(TEvent eventData, CancellationToken cancellationToken = default)
            where TEvent : TAsyncEvent
        {
            if (!_asyncEvents.TryGetValue(typeof(TEvent), out var asyncEventDelegateContainer))
                return;

            try
            {
                await asyncEventDelegateContainer.Invoke(eventData, cancellationToken);
            }
            catch
            {
                Debug.Log($"[EventModule::NotifyAsyncEvent] AsyncEvent: {typeof(TEvent)} is canceled.");
            }
        }

        public UniTask NotifyAsyncEvent<TEvent>(TEvent eventData, out CancellationTokenSource cts)
            where TEvent : TAsyncEvent
        {
            cts = new CancellationTokenSource();
            return NotifyAsyncEvent(eventData, cts.Token);
        }


        //private method
        private void AddListener<TEvent, TEventDelegateContainer>(Delegate eventDelegate,
            Dictionary<Type, TEventDelegateContainer> events)
            where TEvent : TBaseEvent where TEventDelegateContainer : BaseEventDelegateContainer, new()
        {
            var type = typeof(TEvent);

            if (!events.TryGetValue(type, out var eventDelegateContainer))
            {
                eventDelegateContainer = new TEventDelegateContainer();
                eventDelegateContainer.SetEventDelegate(eventDelegate);
                events.Add(type, eventDelegateContainer);
            }
            else
            {
                eventDelegate = (Action<TEvent>)Delegate.Combine(eventDelegateContainer.EventDelegate, eventDelegate);
                eventDelegateContainer.SetEventDelegate(eventDelegate);
            }
        }

        private void RemoveListener<TEvent, TEventDelegateContainer>(Delegate eventDelegate,
            Dictionary<Type, TEventDelegateContainer> events)
            where TEvent : TBaseEvent where TEventDelegateContainer : BaseEventDelegateContainer, new()
        {
            var type = typeof(TEvent);

            if (events.TryGetValue(type, out var eventDelegateContainer))
            {
                eventDelegate = Delegate.Remove(eventDelegateContainer.EventDelegate, eventDelegate);
                if (eventDelegate is null || eventDelegate.GetInvocationList().Length <= 0)
                    events.Remove(type);

                else
                    eventDelegateContainer.SetEventDelegate(eventDelegate);
            }
        }
    }
}