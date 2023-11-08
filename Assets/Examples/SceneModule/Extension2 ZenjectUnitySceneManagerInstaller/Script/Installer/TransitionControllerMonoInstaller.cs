// using CizaSceneModule;
// using CizaSceneModule.Implement;
// using UnityEngine;
// using Zenject;
//
//
// public class TransitionControllerMonoInstaller : MonoInstaller
// {
//     [SerializeField] private TransitionControllerConfig _transitionControllerConfig;
//
//     public override void InstallBindings()
//     {
//         Container.Bind<ITransitionControllerConfig>().FromInstance(_transitionControllerConfig);
//         Container.Bind<TransitionController>().AsSingle().NonLazy();
//     }
// }