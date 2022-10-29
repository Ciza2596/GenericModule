
namespace ViewModule
{
    public abstract class FadeOutViewBase : ViewBase
    {
        //private variable
        private ViewModule _viewModule;

        private string _selfViewName;

        private bool _once;
        
        //protected variable
        protected bool _isCompletedFadeOut;


        //baseView callback
        protected override void OnInit(params object[] items)
        {
            if (items is null || items.Length <= 0)
                return;

            if (items[0] is ViewModule viewModule)
                _viewModule = viewModule;
        }

        protected override void OnShow(params object[] items)
        {
            base.OnShow(items);

            _once = true;
            _isCompletedFadeOut = false;
            
            
            if(items.Length < 1)
                return;
            
            if (items[0] is string selfViewName)
                _selfViewName = selfViewName;
        }

        protected override void OnHide()
        {
            base.OnHide();
        }


        protected override void OnRelease() { }

        protected override void OnVisibleUpdateStart(float deltaTime)
        {
            if (!_once || !_isCompletedFadeOut)
                return;

            _once = false;
            
            _viewModule.HideView(_selfViewName);
        }

        protected override void OnUpdateStart(float deltaTime) { }
    }
}