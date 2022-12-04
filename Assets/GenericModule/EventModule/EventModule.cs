using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventModule
{
    public abstract class EventModule<TBaseEvent, TSyncEvent, TAsyncEvent>
        where TSyncEvent : TBaseEvent where TAsyncEvent : TBaseEvent
    {
        //private method
        private Dictionary<Type, SyncEventDelegateContainer> _syncEvents =
            new Dictionary<Type, SyncEventDelegateContainer>();

        private Dictionary<Type, AsyncEventDelegateContainer> _asyncEvents =
            new Dictionary<Type, AsyncEventDelegateContainer>();


        //public variable
        public IReadOnlyDictionary<Type, SyncEventDelegateContainer> SyncEvents => _syncEvents;
        public IReadOnlyDictionary<Type, AsyncEventDelegateContainer> AsyncEvents => _asyncEvents;


        //public method

        //sync
        public void AddSyncListener<TEvent>(Action<TEvent> eventDelegate) where TEvent : TSyncEvent =>
            AddListener<TEvent, SyncEventDelegateContainer>(eventDelegate, _syncEvents);

        public void RemoveSyncListener<TEvent>(Action<TEvent> eventDelegate) where TEvent : TSyncEvent =>
            RemoveListener<TEvent, SyncEventDelegateContainer>(eventDelegate, _syncEvents);


        public void NotifySyncEvent<TEvent>(TEvent eventData = default) where TEvent : TSyncEvent
        {
            if (_syncEvents.TryGetValue(typeof(TEvent), out var syncEventDelegateContainer))
                syncEventDelegateContainer?.Invoke(eventData);
        }


        //async
        public void AddAsyncListener<TEvent>(Func<TEvent, Task> eventDelegate) where TEvent : TAsyncEvent =>
            AddListener<TEvent, AsyncEventDelegateContainer>(eventDelegate, _asyncEvents);

        public void RemoveAsyncListener<TEvent>(Func<TEvent, Task> eventDelegate) where TEvent : TAsyncEvent =>
            RemoveListener<TEvent, AsyncEventDelegateContainer>(eventDelegate, _asyncEvents);


        public async Task NotifyAsyncEvent<TEvent>(TEvent eventData = default) where TEvent : TAsyncEvent
        {
            if (_asyncEvents.TryGetValue(typeof(TEvent), out var asyncEventDelegateContainer))
                await asyncEventDelegateContainer?.Invoke(eventData);
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