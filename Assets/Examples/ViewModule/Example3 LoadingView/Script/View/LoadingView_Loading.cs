using UnityEngine;

namespace ViewModule.Example3
{
    public class LoadingView_Loading : BaseLoadingView
    {
        //private variable
        [SerializeField] private int _loadingTime;
        
        private float _currentLoadingTime;


        //baseView callback
        protected override void OnShow(params object[] parameters)
        {
            base.OnShow(parameters);

            _currentLoadingTime = _loadingTime;
        }
        

        protected override void OnVisibleTick(float deltaTime)
        {
            base.OnVisibleTick(deltaTime);
            _currentLoadingTime -= deltaTime;

            if (_currentLoadingTime <= 0 && !_isCompleting)
                _isCompleting = true;

        }

        protected override void OnTick(float deltaTime) { }
    }
}