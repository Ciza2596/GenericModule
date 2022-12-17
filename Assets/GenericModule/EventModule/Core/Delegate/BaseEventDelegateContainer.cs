using System;

namespace EventModule
{
    public class BaseEventDelegateContainer
    {
        public Delegate EventDelegate { get; private set; }

        public void SetEventDelegate(Delegate eventDelegate) => EventDelegate = eventDelegate;
    }
}