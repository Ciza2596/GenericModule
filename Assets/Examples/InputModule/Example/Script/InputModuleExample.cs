using System;
using CizaInputModule;
using CizaInputModule.Implement;
using UnityEngine;

public class InputModuleExample : MonoBehaviour
{
    [SerializeField]
    private InputModuleConfig _inputModuleConfig;

    private InputModule _inputModule;

    private void OnEnable()
    {
        _inputModule = new InputModule(_inputModuleConfig);

        _inputModule.Initialize();

        _inputModule.StartJoining(2);
        _inputModule.EnableInput();
    }

    private void Update()
    {
        _inputModule?.Tick(Time.deltaTime);
    }

    private void OnDisable()
    {
        _inputModule.Release();
    }
}