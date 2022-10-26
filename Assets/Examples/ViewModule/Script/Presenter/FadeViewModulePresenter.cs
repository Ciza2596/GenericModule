using Zenject;

namespace ViewModule.Example
{
    public class FadeViewModulePresenter: IInitializable
    {
        private ViewModule _viewModule;
        
        
        //zenject callback
        public void Initialize()
        {
            _viewModule.LoadView(ViewConfig.TITLE_NAME, _viewModule);
            _viewModule.ShowView(ViewConfig.TITLE_NAME);
            _viewModule.LoadView(ViewConfig.LOBBY_NAME, _viewModule);
            _viewModule.LoadView(ViewConfig.FADE_NAME, _viewModule);
        }
        
        //public method
        public FadeViewModulePresenter(ViewModule viewModule)
        {
            _viewModule = viewModule;
        }

    }
}


