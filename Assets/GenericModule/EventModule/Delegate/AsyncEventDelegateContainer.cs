using System;
using System.Threading.Tasks;

namespace EventModule
{
    public class AsyncEventDelegateContainer : BaseEventDelegateContainer
    {
        public async Task Invoke<T1>(T1 eventData)
        {
            await ((Func<T1, Task>)EventDelegate)?.Invoke(eventData);
        }
    }
}