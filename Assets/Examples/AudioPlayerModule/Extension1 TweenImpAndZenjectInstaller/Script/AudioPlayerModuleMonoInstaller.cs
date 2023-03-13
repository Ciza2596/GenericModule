using CizaAudioPlayerModule;
using CizaAudioPlayerModule.Implement;
using Zenject;

public class AudioPlayerModuleMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<CizaAudioPlayerModule.AudioPlayerModule>().AsSingle();
        Container.Bind<ITween>().To<TweenImp>().AsSingle();
    }
}