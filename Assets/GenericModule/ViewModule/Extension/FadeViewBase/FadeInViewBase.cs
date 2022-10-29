
using System;

namespace ViewModule
{
    public abstract class FadeInViewBase : ViewBase
    {
        //private variable
        private ViewModule _viewModule;

        private string _selfViewName;
        private string _hideViewName;
        private Action _onCompletedAction;

        private bool _once;
        
        //protected variable
        protected bool _isCompletedFadeIn;


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
            _isCompletedFadeIn = false;
            
            
            if(items.Length != 3)
                return;
            
            if (items[0] is string selfViewName)
                _selfViewName = selfViewName;

            if (items[1] is string hideViewName)
                _hideViewName = hideViewName;

            if (items[2] is Action onCompletedAction)
                _onCompletedAction = onCompletedAction;
        }

        protected override void OnHide()
        {
            base.OnHide();
        }


        protected override void OnRelease() { }

        protected override void OnVisibleUpdateStart(float deltaTime)
        {
            if (!_once || !_isCompletedFadeIn)
                return;

            _once = false;
            
            _viewModule.HideView(_hideViewName);
            _viewModule.HideView(_selfViewName , _onCompletedAction);
        }

        protected override void OnUpdateStart(float deltaTime) { }
    }
}