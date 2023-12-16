## Info
Modules for unity.

module | Test | Samples |
--- | :---: | :---: |
[LogModule](#logmodule) | ✔️ | ❌ |
[PageModule](#pagemodule) | ✔️ | ❌ |
[TimerModule](#timermodule) | ✔️ | ❌ |
[AudioModule](#audiomodule) | ✔️ | ✔️ |
[EventModule](#eventmodule) | ✔️ | ❌ |
[SaveLoadModule](#saveloadmodule) | ✔️ | ❌ |
[ObjectPool](#objectpool) | ✔️ | ❌ |
[GameObjectPoolModule](#gameobjectpoolmodule) | ✔️ | ❌ |
[LocalizationModule](#localizationmodule) | ✔️ | ❌ |
[TextModule](#textmodule) | ✔️ | ❌ |
[InputModule](#inputModule) | ✔️ | ❌ |
[OptionModule](#optionmodule) | ❌ | ❌ |
[SceneModule](#scenemodule) | ❌ | ❌ |
[AddressablesModule](#addressablesmodule) | ❌ | ❌ |
[LocaleAddressablesModule](#localeaddressablesmodule) | ❌ | ❌ |

## LogModule
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/LogModule
```

### Manual:

- **Operate LogModuleConfig**
  1. Create LogModuleConfig:
    
     <img src="Document/LogModule/Image/CreateLogModuleConfig.png"/>
    
  2. LogModuleConfig Inspector:
    
     <img src="Document/LogModule/Image/LogModuleConfigInspector.png"/>
     
- **Example**
```csharp
using CizaLogModule;
using CizaLogModule.Implement;
using UnityEngine;

public class LogModuleExample : MonoBehaviour
{
    [SerializeField]
    private LogModuleConfig _logModuleConfig;
    
    private void Awake()
    {
        var logModule = new LogModule(_logModuleConfig, new UnityLogPrinter());
        logModule.Debug("Hello World!");
    }
}
```



## PageModule
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/PageModule
```
### Dependency:
```
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
```

### Manual:
Dependency [UniTask](https://github.com/Cysharp/UniTask)

PageModule manual.


## TimerModule
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/TimerModule
```

### Manual:
- **Example**
```csharp
using CizaTimerModule;
using UnityEngine;

public class TimerModuleExample : MonoBehaviour
{
    private TimerModule _timerModule;

    // unity callback
    private void Awake()
    {
        _timerModule = new TimerModule();
        _timerModule.Initialize();

        var timerId = _timerModule.AddLoopTimer(1, SayHello); // Call SayHello method every second.
        _timerModule.RemoveTimer(timerId);
    }

    private void Update() =>
        _timerModule.Tick(Time.deltaTime);

    // private method
    private void SayHello(ITimerReadModel timerReadModel) =>
        Debug.Log("Hello!");
}
```

## AudioModule
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/AudioModule
```
### Dependency:
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/TimerModule
```
```
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
```

### Manual:
Dependency [UniTask](https://github.com/Cysharp/UniTask)

- **Operate AudioModuleConfig**
  1. Create AudioModuleConfig:
    
     <img src="Document/AudioModule/Image/CreateAudioModuleConfig.png"/>
    
  2. AudioModuleConfig Inspector:
    
     <img src="Document/AudioModule/Image/AudioModuleConfigInspector.png"/>
     
  3. AudioModuleAssetProvider Inspector:
    
     <img src="Document/AudioModule/Image/AudioModuleAssetProviderInspector.png"/>
     
  4. AudioDataOverview Inspector:
    
     <img src="Document/AudioModule/Image/AudioDataOverviewInspector.png"/>
     
- **Example**
```csharp
using CizaAudioModule;
using CizaAudioModule.Implement;
using UnityEngine;
using UnityEngine.Audio;

public class AudioModuleExample : MonoBehaviour
{
    [SerializeField] private AudioDataOverview _audioDataOverview;
    [Space] 
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioModuleConfig _audioModuleConfig;
    [SerializeField] private AudioModuleAssetProvider _audioModuleAssetProvider;

    private AudioModule _audioModule;
    
    private async void Awake()
    {
        _audioModule = new AudioModule(_audioModuleConfig, _audioModuleAssetProvider, _audioMixer);

        var audioDataMap = _audioDataOverview.GetAudioDataMap();
        await _audioModule.Initialize(audioDataMap);

        var audioId = _audioModule.Play("wind_bell");
        _audioModule.Stop(audioId);
    }
}
```


## EventModule
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/EventModule
```
### Dependency:
```
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
```

### Manual:
Dependency [UniTask](https://github.com/Cysharp/UniTask)

- **Example**
```csharp
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
```


## SaveLoadModule
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/SaveLoadModule
```

### Manual:
Can use on field and property. Not include - public/protected/private type propertyName { get; }

Support type:
  1. Primitive: bool, char, dateTime, double, enum, float, int, long, short, string
  2. Collection: array, array2D, array3D, dictionary, hashset, list, queue, stack
  3. Unity: vector2, vector2Int, vector3, vector3Int

- **Example**

- **Operate SaveModuleConfig**
    1. Create SaveModuleConfig:
    
       <img src="Document/LogModule/Image/CreateLogModuleConfig.png"/>
    
    2. SaveModuleConfig Inspector:
    
       <img src="Document/LogModule/Image/LogModuleInspector.png"/>
       
       
## ObjectPool
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/ObjectPool
```

### Manual:
- **Example**
```csharp
using System;
using CizaObjectPool;
using UnityEngine;

public class ObjectPoolExample : MonoBehaviour
{
    private readonly ObjectPool<Character> _characterPool = new ObjectPool<Character>();

    // unity callback
    private void Awake()
    {
        var character = _characterPool.Spawn();
        character.Initialize("Ciza", 100);
        
        _characterPool.DeSpawn(character); // call Dispose method
    }

    private void OnApplicationQuit()
    {
        if(_characterPool.HasPool)
            _characterPool.Release();
    }

    private class Character : IDisposable
    {
        public string Name { get; private set; }
        public float Hp { get; private set; }
        public GameObject Body { get; private set; }

        public void Initialize(string name, float hp)
        {
            Name = name;
            Hp = hp;
            Body = new GameObject(Name);
        }

        public void Dispose()
        {
            Name = string.Empty;
            Hp = 0;
            var body = Body;
            Body = null;
            Destroy(body);
        }
    }
}
```


## GameObjectPoolModule
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/GameObjectPoolModule
```

### Manual:
GameObjectPoolModule manual.


## LocalizationModule
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/LocalizationModule
```

### Dependency:
```
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
```

### Manual:
LocalizationModule manual.

## TextModule
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/TextModule
```

### Manual:
TextModule manual.

## InputModule
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/InputModule
```

### Dependency:
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/TimerModule
```
```
Unity.InputSystem
```

### Manual:
InputModule manual.


## OptionModule
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/OptionModule
```

### Dependency:
```
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
```
```
https://github.com/Ciza2596/CizaCore.git?path=Assets/_Project/CizaCore
```
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/PageModule
```

### Manual:
OptionModule manual.


## SceneModule
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/SceneModule
```

### Manual:
SceneModule manual.

## AddressablesModule
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/AddressablesModule
```
### Dependency:
```
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
```

### Manual:
Dependency [UniTask](https://github.com/Cysharp/UniTask) and [Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@1.21/manual/index.html).

- **Example**
```csharp
public class AddressablesModuleExample : MonoBehaviour
{
    private void Awake()
    {
    }
}
```
- **Operate Editor**
    1. Open AddressblesAssetManager:
    
       <img src="Document/LogModule/Image/CreateLogModuleConfig.png"/>
    
    2. Export Page:
    
       <img src="Document/LogModule/Image/LogModuleInspector.png"/>
       
    3. Import Page:
    
       <img src="Document/LogModule/Image/LogModuleInspector.png"/>
       
    4. Add Page:
    
       <img src="Document/LogModule/Image/LogModuleInspector.png"/>

## LocaleAddressablesModule
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/LocaleAddressablesModule
```
### Dependency:
```
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
```
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/AddressablesModule
```
```
https://github.com/Ciza2596/GenericModule.git?path=Assets/GenericModule/LocalizationModule
```
       
