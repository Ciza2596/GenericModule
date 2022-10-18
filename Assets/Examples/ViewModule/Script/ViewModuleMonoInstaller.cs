using GameCore.Generic.Implement.Derived;
using UnityEngine;
using Zenject;

namespace ViewModule.Example
{
    public class ViewModuleMonoInstaller : MonoInstaller
    {
        [SerializeField] private ViewDataOverview _viewDataOverview;


        public override void InstallBindings()
        {
            Container.BindInstance(_viewDataOverview);
            Container.BindInterfacesAndSelfTo<TimeProvider>();
            Container.BindInterfacesAndSelfTo<ViewModuleAdapter>();
        }
    }
}