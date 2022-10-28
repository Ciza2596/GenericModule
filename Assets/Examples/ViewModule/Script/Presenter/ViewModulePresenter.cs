using Zenject;

namespace ViewModule.Example
{
    public class ViewModulePresenter: IInitializable
    {
        private ViewModule _viewModule;
        
        
        //zenject callback
        public void Initialize()
        {
            _viewModule.LoadView(ViewTypes.Title.ToString(), _viewModule);
            _viewModule.ShowView(ViewTypes.Title.ToString());
            _viewModule.LoadView(ViewTypes.Lobby.ToString(), _viewModule);
        }
        
        //public method
        public ViewModulePresenter(ViewModule viewModule)
        {
            _viewModule = viewModule;
        }

    }
}


