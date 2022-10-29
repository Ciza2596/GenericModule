using Zenject;

namespace ViewModule.Example2
{
    public class AnimViewModulePresenter: IInitializable
    {
        private ViewModule _viewModule;
        
        
        //zenject callback
        public void Initialize()
        {
            _viewModule.LoadView(AnimViewTypes.Title.ToString(), _viewModule);
            _viewModule.ShowView(AnimViewTypes.Title.ToString());
            _viewModule.LoadView(AnimViewTypes.Lobby.ToString(), _viewModule);
        }
        
        //public method
        public AnimViewModulePresenter(ViewModule viewModule)
        {
            _viewModule = viewModule;
        }

    }
}


