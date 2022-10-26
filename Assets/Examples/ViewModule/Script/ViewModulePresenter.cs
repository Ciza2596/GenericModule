using Zenject;

namespace ViewModule.Example
{
    public class ViewModulePresenter: IInitializable
    {
        private ViewModule _viewModule;
        
        
        //zenject callback
        public void Initialize()
        {
            _viewModule.LoadView(ViewConfig.TITLE_NAME, _viewModule);
            _viewModule.ShowView(ViewConfig.TITLE_NAME);
            _viewModule.LoadView(ViewConfig.LOBBY_NAME, _viewModule);
        }
        
        //public method
        public ViewModulePresenter(ViewModule viewModule)
        {
            _viewModule = viewModule;
        }

    }
}


