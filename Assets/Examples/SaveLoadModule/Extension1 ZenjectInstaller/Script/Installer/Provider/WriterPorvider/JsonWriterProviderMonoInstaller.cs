using SaveLoadModule;
using SaveLoadModule.Implement;
using Zenject;

public class JsonWriterProviderMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IWriterProvider>().To<JsonWriteProvider>().AsSingle();
    }
}