using Zenject;

namespace ViewModule.Example3
{
    public class FadeViewModulePresenter: IInitializable
    {
        private ViewModule _viewModule;
        
        
        //zenject callback
        public void Initialize()
        {
            _viewModule.LoadView(FadeViewTypes.Title.ToString(), _viewModule);
            _viewModule.LoadView(FadeViewTypes.Lobby.ToString(), _viewModule);
            _viewModule.LoadView(FadeViewTypes.FadeIn.ToString(), _viewModule);
            _viewModule.LoadView(FadeViewTypes.Loading.ToString(), _viewModule);
            _viewModule.LoadView(FadeViewTypes.FadeOut.ToString(), _viewModule);
            
            _viewModule.ShowView(FadeViewTypes.Title.ToString());
        }
        
        //public method
        public FadeViewModulePresenter(ViewModule viewModule)
        {
            _viewModule = viewModule;
        }

    }
}


