using UnityEngine;
using Zenject;

namespace SceneModule.Example1
{
    public class SceneMonoInstaller : MonoInstaller
    {

        [SerializeField] private TransitionSceneData _transitionSceneData;
        [SerializeField] private ComponentCollection _componentCollection;

        public override void InstallBindings()
        {
            Container.Bind<TransitionSceneData>().FromInstance(_transitionSceneData);
            Container.Bind<ComponentCollection>().FromInstance(_componentCollection);
            Container.BindInterfacesAndSelfTo<ComponentController>().AsSingle();
        }
    }
}