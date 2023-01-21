using SaveLoadModule;
using SaveLoadModule.Implement;
using Zenject;

public class JsonReaderProviderMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IReaderProvider>().To<JsonReaderProvider>().AsSingle();
    }
}