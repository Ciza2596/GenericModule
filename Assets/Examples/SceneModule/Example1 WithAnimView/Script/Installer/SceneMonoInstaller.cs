using UnityEngine;
using Zenject;

namespace SceneModule.Example1
{
    public class SceneMonoInstaller : MonoInstaller
    {
        [SerializeField] private TransitionSceneData _transitionSceneData;
        [Space] [SerializeField] private ComponentCollectionData _componentCollectionData;

        public override void InstallBindings()
        {
            Container.Bind<TransitionSceneData>().FromInstance(_transitionSceneData);
            Container.Bind<ComponentCollectionData>().FromInstance(_componentCollectionData);
            Container.BindInterfacesAndSelfTo<ComponentController>().AsSingle();
        }
    }
}