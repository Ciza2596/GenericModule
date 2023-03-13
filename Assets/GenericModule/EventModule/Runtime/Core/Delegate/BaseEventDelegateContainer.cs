using System;

namespace CizaEventModule
{
    internal  class BaseEventDelegateContainer
    {
        public Delegate EventDelegate { get; private set; }

        public void SetEventDelegate(Delegate eventDelegate) => EventDelegate = eventDelegate;
    }
}