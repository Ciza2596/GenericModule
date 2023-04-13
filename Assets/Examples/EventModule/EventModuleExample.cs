using System.Threading;
using CizaEventModule;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EventModuleExample : MonoBehaviour
{
    private readonly EventModuleImp _eventModuleImp = new EventModuleImp();
    
    // unity callback
    private void OnEnable()
    {
        _eventModuleImp.AddSyncListener<SayHelloSyncEvent>(OnSayHelloSyncEvent);
        _eventModuleImp.AddAsyncListener<SayHelloAsyncEvent>(OnSayHelloAsyncEvent);
    }


    private void OnDisable()
    {
        _eventModuleImp.RemoveSyncListener<SayHelloSyncEvent>(OnSayHelloSyncEvent);
        _eventModuleImp.RemoveAsyncListener<SayHelloAsyncEvent>(OnSayHelloAsyncEvent);
    }

    private async void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
            _eventModuleImp.NotifySyncEvent(new SayHelloSyncEvent());
        
        if(Input.GetKeyDown(KeyCode.A))
            await _eventModuleImp.NotifyAsyncEvent(new SayHelloAsyncEvent());
    }
    
    // event callback
    private void OnSayHelloSyncEvent(SayHelloSyncEvent sayHelloSyncEvent) =>
        Debug.Log("Hello! SyncEvent!");

    private async UniTask OnSayHelloAsyncEvent(SayHelloAsyncEvent sayHelloAsyncEvent, CancellationToken cancellationToken)
    {
        Debug.Log("Hello! AsyncEvent!");
        await UniTask.CompletedTask;
    }

    // eventModuleImp
    private interface IEvent { }
    private interface ISyncEvent: IEvent { }
    private interface IAsyncEvent: IEvent { }
    private class EventModuleImp: EventModule<IEvent, ISyncEvent, IAsyncEvent> { }
    
    // event
    private class SayHelloSyncEvent: ISyncEvent { }
    private class SayHelloAsyncEvent: IAsyncEvent { }
}
