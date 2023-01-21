using SaveLoadModule;
using SaveLoadModule.Implement;
using Zenject;

public class FileStreamProviderMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IStreamProvider>().To<FileStreamProvider>().AsSingle();
    }
}