using SaveLoadModule;
using SaveLoadModule.Implement;
using UnityEngine;
using Zenject;

public class FileStreamProviderMonoInstaller : MonoInstaller
{
    [SerializeField] private SaveLoadModuleConfig _saveLoadModuleConfig;

    public override void InstallBindings()
    {
        Container.Bind<IStreamProvider>().To<FileStreamProvider>().AsSingle();
        Container.Bind<IFileStreamProviderConfig>().To<SaveLoadModuleConfig>()
            .FromInstance(_saveLoadModuleConfig);
    }
}