using UnityEngine;

namespace ViewModule.Example
{
    public class FadeView_Loading : LoadingViewBase
    {
        //private variable
        [SerializeField] private int _loadingTime;
        
        private float _currentLoadingTime;


        //baseView callback
        protected override void OnInit(params object[] items)
        {
            base.OnInit(items);
        }

        protected override void OnShow(params object[] items)
        {
            base.OnShow(items);

            _currentLoadingTime = _loadingTime;
        }

        protected override void OnHide()
        {
            base.OnHide();
        }


        protected override void OnRelease() { }

        protected override void OnVisibleUpdateStart(float deltaTime)
        {
            base.OnVisibleUpdateStart(deltaTime);
            _currentLoadingTime -= deltaTime;

            if (_currentLoadingTime <= 0 && !_isCompletedLoading)
                _isCompletedLoading = true;

        }

        protected override void OnUpdateStart(float deltaTime) { }
    }
}