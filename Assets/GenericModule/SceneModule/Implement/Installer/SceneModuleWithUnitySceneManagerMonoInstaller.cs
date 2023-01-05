using UnityEngine;
using Zenject;

namespace SceneModule.Implement
{
    public class SceneModuleWithUnitySceneManagerMonoInstaller : MonoInstaller
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
