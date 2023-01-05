using UnityEngine;
using Zenject;

namespace SceneModule.Implement
{
    public class TransitionControllerMonoInstaller : MonoInstaller
    {
        [SerializeField]
        private TransitionControllerConfig _transitionControllerConfig;
        
        public override void InstallBindings()
        {
            Container.Bind<ITransitionControllerConfig>().FromInstance(_transitionControllerConfig);
            Container.Bind<TransitionController>().AsSingle().NonLazy();
        }
    }
}