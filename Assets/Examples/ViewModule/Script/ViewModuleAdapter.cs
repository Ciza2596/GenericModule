using System;
using GameCore.Generic.Infrastructure;
using Zenject;

namespace ViewModule.Example
{
    public class ViewModuleAdapter: ITickable, IViewModule
    {
        //private variable
        private readonly ViewModule _viewModule;

        
        //zenject callback
        public void Tick()
        {
            _viewModule.Tick();
        }

        //public method

        public ViewModuleAdapter(ITimeProvider timeProvider, IViewDataOverview viewDataOverview)
        {
            _viewModule = new ViewModule(timeProvider, viewDataOverview);
        }
        

        public T GetViewComponent<T>(string viewName) => _viewModule.GetViewComponent<T>(viewName);

        public bool GetIsVisibleView(string viewName) => _viewModule.GetIsVisibleView(viewName);
        public bool GetIsShowing(string viewName) => _viewModule.GetIsShowing(viewName);
        public bool GetIsHiding(string viewName) => _viewModule.GetIsHiding(viewName);


        public void LoadView(string viewName, params object[] items) => _viewModule.LoadView(viewName, items);
        public void LoadAllViews() => _viewModule.LoadAllViews();


        public void ReleaseView(string viewName) => _viewModule.ReleaseView(viewName);
        public void ReleaseAllViews() => _viewModule.ReleaseAllViews();


        public void ShowView(string viewName, params object[] items) => _viewModule.ShowView(viewName, items);
        public void HideView(string viewName, Action onCompletedAction = null) => _viewModule.HideView(viewName, onCompletedAction);

    }
    
}
