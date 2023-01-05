using SceneModule.Implement;
using UnityEngine;
using Zenject;

namespace SceneModule.Example2
{
    public class SceneModuleWithAddressablesModuleSceneManagerMonoInstaller : MonoInstaller
    {
        [SerializeField] public SceneModuleConfig _sceneModuleConfig;

        public override void InstallBindings()
        {
            Container.Bind<AddressablesModule.AddressablesModule>().AsSingle();
            Container.Bind<ISceneModuleConfig>().FromInstance(_sceneModuleConfig);
            Container.BindInterfacesAndSelfTo<AddressablesModuleSceneManager>().AsSingle();
            Container.Bind<SceneModule>().AsSingle();
        }
    }
}