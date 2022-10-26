
using UnityEngine;
using ViewModule.Example;

namespace ViewModule
{
    public class FadeView_Fade : ViewBase
    {
        //private variable
        [SerializeField] private int _loadingTime;


        private float _currentLoadingTime;
        
        private ViewModule _viewModule;

        private string _nextViewName;
        private object[] _items;
        

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
            base.OnShow();

            if (items[0] is string nextViewName)
                _nextViewName = nextViewName;

            var length = items.Length;
            _items = new object[length - 1];

            if (length > 1)
            {
                for (int i = 1; i < length; i++)
                    items[i] = _items[i];
            }

            _currentLoadingTime = _loadingTime;
        }

        protected override void OnHide()
        {
            base.OnHide();
        }


        protected override void OnRelease() { }

        protected override void OnVisibleUpdateStart(float deltaTime)
        {
            _currentLoadingTime -= deltaTime;

            if (_currentLoadingTime <= 0)
                _viewModule.HideView(ViewConfig.FADE_NAME, () => _viewModule.ShowView(_nextViewName, _items));
            
        }

        protected override void OnUpdateStart(float deltaTime) { }
    }
}