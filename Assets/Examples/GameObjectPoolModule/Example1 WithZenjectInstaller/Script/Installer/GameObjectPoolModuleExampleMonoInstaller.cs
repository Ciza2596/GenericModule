// using UnityEngine;
// using Zenject;
//
// namespace CizaGameObjectPoolModule.Example1
// {
//     public class GameObjectPoolModuleExampleMonoInstaller : MonoInstaller
//     {
//         [SerializeField]
//         private ComponentCollectionData _componentCollectionData;
//         
//         public override void InstallBindings()
//         {
//             Container.BindInterfacesAndSelfTo<GameObjectPoolModuleExampleController>().AsSingle().NonLazy();
//             Container.Bind<ComponentCollectionData>().FromInstance(_componentCollectionData);
//         }
//     }
// }