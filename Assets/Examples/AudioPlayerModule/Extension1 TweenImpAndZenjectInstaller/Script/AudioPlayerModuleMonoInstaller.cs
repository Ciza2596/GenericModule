using AudioPlayerModule;
using AudioPlayerModule.Implement;
using Zenject;

public class AudioPlayerModuleMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<AudioPlayerModule.AudioPlayerModule>().AsSingle();
        Container.Bind<ITween>().To<TweenImp>().AsSingle();
    }
}