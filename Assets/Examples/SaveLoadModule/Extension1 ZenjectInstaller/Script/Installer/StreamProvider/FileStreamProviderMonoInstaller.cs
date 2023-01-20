using SaveLoadModule;
using SaveLoadModule.Implement;
using UnityEngine;
using Zenject;

public class FileStreamProviderMonoInstaller : MonoInstaller
{
    [SerializeField] private FileStreamProviderConfig _fileStreamProviderConfig;

    public override void InstallBindings()
    {
        Container.Bind<IStreamProvider>().To<FileStreamProvider>().AsSingle();
        Container.Bind<IFileStreamProviderConfig>().To<FileStreamProviderConfig>()
            .FromInstance(_fileStreamProviderConfig);
    }
}