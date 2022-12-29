using SceneModule.Implement;
using UnityEngine;
using Zenject;

namespace SceneModule.Example1
{
    public class SceneModuleMonoInstaller : MonoInstaller
    {
        [SerializeField] public SceneModuleConfig _sceneModuleConfig;
        
        public override void InstallBindings()
        {
            Container.Bind<ISceneModuleConfig>().FromInstance(_sceneModuleConfig);
            Container.BindInterfacesAndSelfTo<UnitySceneManager>().AsSingle();
            Container.Bind<SceneModule>().AsSingle();
        }
    }
}
