using System;
using UnityEngine;

namespace SceneModule.Example1
{
    public class BaseLoadingView : MonoBehaviour, ILoadingView
    {
        //private variable
        [SerializeField] private float _defaultLoadingTime = 1;
        
        private TransitionController _transitionController;
        private Action _onComplete;

        private float _loadingTime;
        private bool _isOnce;
        
        
        //loadingView callback
        public virtual void Loading(TransitionController transitionController, Action onComplete)
        {
            gameObject.SetActive(true);
            
            _transitionController = transitionController;
            _onComplete = onComplete;

            _loadingTime = _defaultLoadingTime;
            _isOnce = true;
        }

        
        //unity callback
        private void Update()
        {
            if(!_isOnce)
                return;
            
            var loadingProgress = _transitionController.LoadingProgress;
            
            if (_isOnce && loadingProgress >= 0.9f && _loadingTime <= 0)
            {
                _isOnce = false;
                _onComplete?.Invoke();
                gameObject.SetActive(false);
                return;
            }
            _loadingTime -= Time.deltaTime;
        }
    }
}