using System;
using UnityEngine;

namespace SceneModule.Implement
{
    public class BaseLoadingView : MonoBehaviour, ILoadingView
    {
        //private variable
        [SerializeField] private float _defaultLoadingTime = 1;

        private ILoadSceneAsync _loadSceneAsync;
        private Action _onComplete;

        private float _loadingTime;
        private bool _isOnce;


        //loadingView callback
        public void Loading(ILoadSceneAsync loadSceneAsync, Action onComplete)
        {
            gameObject.SetActive(true);

            _loadSceneAsync = loadSceneAsync;
            _onComplete = onComplete;

            _loadingTime = _defaultLoadingTime;
            _isOnce = true;
        }


        //unity callback
        private void Update()
        {
            if (!_isOnce)
                return;

            if (_isOnce && _loadSceneAsync.IsDone && _loadingTime <= 0)
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