using System;
using UnityEngine;

namespace SceneModule.Implement
{
    public class BaseLoadingView : MonoBehaviour, ILoadingView
    {
        //private variable
        [SerializeField] private float _defaultLoadingTime = 1;

        private ILoadSceneAsync _loadSceneAsync;
        private ISceneTask _loadingTask;
        private Action _onComplete;

        private float _loadingTime;
        private bool _isOnce;


        //loadingView callback
        public void Loading(ILoadSceneAsync loadSceneAsync,ISceneTask loadingTask, Action onComplete)
        {
            gameObject.SetActive(true);

            _loadSceneAsync = loadSceneAsync;
            _loadingTask = loadingTask;
            _onComplete = onComplete;
            
            loadingTask?.Load();

            _loadingTime = _defaultLoadingTime;
            _isOnce = true;
        }


        //unity callback
        private void Update()
        {
            if (!_isOnce)
                return;

            var isCompleteLoading = _loadingTask?.IsCompleteLoading ?? true;

            if (_isOnce && _loadSceneAsync.IsDone && isCompleteLoading && _loadingTime <= 0)
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