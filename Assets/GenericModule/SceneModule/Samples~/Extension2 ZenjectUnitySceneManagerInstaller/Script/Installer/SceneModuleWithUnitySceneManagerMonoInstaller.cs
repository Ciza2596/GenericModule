using SceneModule;
using SceneModule.Implement;
using UnityEngine;
using Zenject;


public class SceneModuleWithUnitySceneManagerMonoInstaller : MonoInstaller
{
    
    [SerializeField] private SceneModuleConfig _sceneModuleConfig;

    public override void InstallBindings()
    {
        Container.Bind<ISceneModuleConfig>().FromInstance(_sceneModuleConfig);
        Container.BindInterfacesAndSelfTo<UnitySceneManager>().AsSingle();
        Container.Bind<SceneModule.SceneModule>().AsSingle();
    }
}