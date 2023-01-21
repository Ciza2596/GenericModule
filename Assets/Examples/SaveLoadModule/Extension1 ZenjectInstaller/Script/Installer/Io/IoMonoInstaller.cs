using SaveLoadModule;
using SaveLoadModule.Implement;
using Zenject;

public class IoMonoInstaller : MonoInstaller
{

    public override void InstallBindings()
    {
        Container.Bind<IIo>().To<Io>().AsSingle();
    }
}