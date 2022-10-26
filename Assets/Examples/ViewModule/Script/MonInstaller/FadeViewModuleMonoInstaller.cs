using GameCore.Generic.Implement.Derived;
using UnityEngine;
using Zenject;

namespace ViewModule.Example
{
    public class FadeViewModuleMonoInstaller : MonoInstaller
    {
        [SerializeField] private ViewDataOverview _viewDataOverview;


        public override void InstallBindings()
        {
            Container.Bind<IViewDataOverview>().FromInstance(_viewDataOverview);
            Container.BindInterfacesAndSelfTo<TimeProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<ViewModule>().AsSingle();
            Container.BindInterfacesAndSelfTo<FadeViewModulePresenter>().AsSingle();
        }
    }
}