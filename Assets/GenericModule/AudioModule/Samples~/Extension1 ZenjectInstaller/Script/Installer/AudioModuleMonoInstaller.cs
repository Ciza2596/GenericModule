using AudioModule;
using AudioModule.Implement;
using UnityEngine;
using Zenject;

public class AudioModuleMonoInstaller : MonoInstaller
{
    [SerializeField]
    private AudioModuleConfig _audioModuleConfig;
    
    public override void InstallBindings()
    {
        Container.Bind<IAudioModuleConfig>().FromInstance(_audioModuleConfig);
        Container.Bind<AudioModule.AudioModule>().AsSingle();
    }
}