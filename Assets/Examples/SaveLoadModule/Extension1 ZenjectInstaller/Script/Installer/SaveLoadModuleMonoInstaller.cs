using DataType;
using SaveLoadModule;
using SaveLoadModule.Implement;
using UnityEngine;
using Zenject;

public class SaveLoadModuleMonoInstaller : MonoInstaller
{
    [SerializeField]
    private SaveLoadModuleConfig _saveLoadModuleConfig;
    
    public override void InstallBindings()
    {
        Container.Bind<SaveLoadModule.SaveLoadModule>().AsSingle().NonLazy();
        Container.Bind<ISaveLoadModuleConfig>().FromInstance(_saveLoadModuleConfig);
        
        Container.Bind<IIo>().To<Io>().AsSingle();
        Container.Bind<IDataTypeController>().To<DataTypeControllerAdapter>().AsSingle();
        
        Container.Bind<IStreamProvider>().To<FileStreamProvider>().AsSingle();
        Container.Bind<IWriterProvider>().To<JsonWriteProvider>().AsSingle();
        Container.Bind<IReaderProvider>().To<JsonReaderProvider>().AsSingle();
        
    }
}