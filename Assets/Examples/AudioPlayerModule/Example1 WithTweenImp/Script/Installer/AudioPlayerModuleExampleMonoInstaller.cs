using GameCore.Implement;
using GameCore.Infrastructure;
using Zenject;

namespace AudioPlayerModule.Example1
{
    public class AudioPlayerModuleExampleMonoInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ITimerModule>().To<TimerModule>().AsSingle();
        }
    }
}