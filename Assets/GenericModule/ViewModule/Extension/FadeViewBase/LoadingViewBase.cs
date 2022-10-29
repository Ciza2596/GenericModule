
namespace ViewModule
{
    public class LoadingViewBase : ViewBase
    {
        //private variable
        private object[] _items;

        private string _selfViewName;

        private string _nextFadeOutViewName;
        private string _nextViewName;

        private ViewModule _viewModule;

        private bool _once;

        //protected variable
        protected bool _isCompletedLoading;


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

            if (items[0] is string selfViewName)
                _selfViewName = selfViewName;

            if (items[1] is string nextFadeOutName)
                _nextFadeOutViewName = nextFadeOutName;

            if (items[2] is string nextViewName)
                _nextViewName = nextViewName;


            var length = items.Length;
            _items = new object[length - 3];

            if (length > 3)
                for (var i = 3; i < length; i++)
                    items[i] = _items[i];

            _once = true;
            _isCompletedLoading = false;
        }

        protected override void OnHide()
        {
            base.OnHide();
        }


        protected override void OnRelease()
        {
        }

        protected override void OnVisibleUpdateStart(float deltaTime)
        {
            if (!_once || !_isCompletedLoading)
                return;

            _once = false;

            _viewModule.HideView(_selfViewName,
                () => _viewModule.ShowViewThenFadeOut(
                    _nextFadeOutViewName, _nextViewName, _items));
        }

        protected override void OnUpdateStart(float deltaTime)
        {
        }
    }
}