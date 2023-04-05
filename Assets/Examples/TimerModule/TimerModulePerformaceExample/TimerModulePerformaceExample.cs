using System;
using CizaTimerModule;
using Sirenix.OdinInspector;
using UnityEngine;

public class TimerModulePerformaceExample : MonoBehaviour
{
    [SerializeField] private int _timerCount = 10000;
    [SerializeField] private float _duration = 0.5f;

    private TimerModule _timerModule = new TimerModule();


    [Button]
    private void AddTimers()
    {
        if (!_timerModule.IsInitialized)
            _timerModule.Initialize();

        for (var i = 0; i < _timerCount; i++)
            _timerModule.AddLoopTimer(_duration, null);
    }

    [Button]
    private void ReleaseTimerModule()
    {
        if (_timerModule.IsInitialized)
            _timerModule.Release();
    }


    private void OnApplicationQuit()
    {
        ReleaseTimerModule();
    }
}