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
    }
}