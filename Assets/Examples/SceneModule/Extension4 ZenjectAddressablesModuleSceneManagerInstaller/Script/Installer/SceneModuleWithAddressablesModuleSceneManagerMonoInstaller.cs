// using CizaSceneModule;
// using CizaSceneModule.Implement;
// using UnityEngine;
// using Zenject;
//
//
// public class SceneModuleWithAddressablesModuleSceneManagerMonoInstaller : MonoInstaller
// {
//     [SerializeField] public SceneModuleConfig _sceneModuleConfig;
//
//     public override void InstallBindings()
//     {
//         Container.Bind<CizaAddressablesModule.AddressablesModule>().AsSingle();
//         Container.Bind<ISceneModuleConfig>().FromInstance(_sceneModuleConfig);
//         Container.BindInterfacesAndSelfTo<AddressablesModuleSceneManager>().AsSingle();
//         Container.Bind<CizaSceneModule.SceneModule>().AsSingle();
//     }
// }