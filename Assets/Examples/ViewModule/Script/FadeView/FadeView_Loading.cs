using UnityEngine;

namespace ViewModule.Example
{
    public class FadeView_Loading : ViewBase
    {
        //private variable
        [SerializeField] private int _loadingTime;


        private float _currentLoadingTime;
        private object[] _items;

        private string _nextFadeOutViewName;
        private string _nextViewName;

        private ViewModule _viewModule;


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

            if (items[0] is string nextFadeOutName)
                _nextFadeOutViewName = nextFadeOutName;
            
            if (items[1] is string nextViewName)
                _nextViewName = nextViewName;


            var length = items.Length;
            _items = new object[length - 2];

            if (length > 2)
                for (var i = 2; i < length; i++)
                    items[i] = _items[i];

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
                _viewModule.HideView(ViewTypes.Loading.ToString(),
                                     () => _viewModule.ShowViewThenFadeOut(
                                         _nextFadeOutViewName, _nextViewName, _items));
        }

        protected override void OnUpdateStart(float deltaTime) { }
    }
}