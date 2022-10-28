using Zenject;

namespace ViewModule.Example
{
    public class FadeViewModulePresenter: IInitializable
    {
        private ViewModule _viewModule;
        
        
        //zenject callback
        public void Initialize()
        {
            _viewModule.LoadView(ViewTypes.Title.ToString(), _viewModule);
            _viewModule.LoadView(ViewTypes.Lobby.ToString(), _viewModule);
            _viewModule.LoadView(ViewTypes.FadeIn.ToString(), _viewModule);
            _viewModule.LoadView(ViewTypes.Loading.ToString(), _viewModule);
            _viewModule.LoadView(ViewTypes.FadeOut.ToString(), _viewModule);
            
            _viewModule.ShowView(ViewTypes.Title.ToString());
        }
        
        //public method
        public FadeViewModulePresenter(ViewModule viewModule)
        {
            _viewModule = viewModule;
        }

    }
}


