using CizaAudioModule;
using CizaAudioModule.Implement;
using UnityEngine;
using Zenject;

public class AudioModuleMonoInstaller : MonoInstaller
{
    [SerializeField]
    private AudioModuleConfig _audioModuleConfig;
    
    public override void InstallBindings()
    {
        Container.Bind<IAudioModuleConfig>().FromInstance(_audioModuleConfig);
        Container.Bind<CizaAudioModule.AudioModule>().AsSingle();
    }
}