// using CizaGameObjectPoolModule;
// using CizaGameObjectPoolModule.Implement;
// using UnityEngine;
// using Zenject;
//
// public class GameObjectPoolModuleMonoInstaller : MonoInstaller
// {
//     [SerializeField] private GameObjectPoolModuleConfig _gameObjectPoolModuleConfig;
//
//
//     public override void InstallBindings()
//     {
//         Container.Bind<CizaGameObjectPoolModule.GameObjectPoolModule>().AsSingle();
//         Container.Bind<IGameObjectPoolModuleConfig>().FromInstance(_gameObjectPoolModuleConfig);
//     }
// }