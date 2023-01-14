using LogModule;
using LogModule.Implement;
using UnityEngine;
using Zenject;

public class LogModuleMonoInstaller : MonoInstaller
{
    [SerializeField] private LogModuleConfig _logModuleConfig;


    public override void InstallBindings()
    {
        Container.Bind<LogModule.LogModule>().AsSingle();
        Container.Bind<ILogModuleConfig>().FromInstance(_logModuleConfig);
    }
}