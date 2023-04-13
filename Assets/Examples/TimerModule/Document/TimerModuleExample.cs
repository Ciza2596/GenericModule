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