using SceneModule;
using SceneModule.Implement;
using UnityEngine;
using Zenject;


public class SceneModuleWithAddressablesModuleSceneManagerMonoInstaller : MonoInstaller
{
    [SerializeField] public SceneModuleConfig _sceneModuleConfig;

    public override void InstallBindings()
    {
        Container.Bind<AddressablesModule.AddressablesModule>().AsSingle();
        Container.Bind<ISceneModuleConfig>().FromInstance(_sceneModuleConfig);
        Container.BindInterfacesAndSelfTo<AddressablesModuleSceneManager>().AsSingle();
        Container.Bind<SceneModule.SceneModule>().AsSingle();
    }
}