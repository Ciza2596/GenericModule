using System;
using UnityEngine;

namespace CizaSceneModule.Implement
{
	public class BaseLoadingView : MonoBehaviour, ILoadingView
	{
		//private variable
		[SerializeField]
		private float _defaultLoadingTime = 1;

		private ILoadSceneAsync   _loadSceneAsync;
		private ILoadingTask      _loadingTask;
		private Action            _activeScene;
		private IInitializingTask _initializingTask;
		private Action            _onComplete;

		private float _loadingTime;
		private bool  _isLoadScene;

		private bool _isLoadInitializingTask;

		//loadingView callback
		public void Loading(ILoadSceneAsync loadSceneAsync, ILoadingTask loadingTask, Action activeScene, IInitializingTask initializingTask, Action onComplete)
		{
			gameObject.SetActive(true);

			_loadSceneAsync   = loadSceneAsync;
			_loadingTask      = loadingTask;
			_activeScene      = activeScene;
			_initializingTask = initializingTask;
			_onComplete       = onComplete;

			loadingTask?.Execute();

			_loadingTime = _defaultLoadingTime;
			_isLoadScene = true;
		}

		//unity callback
		private void Update()
		{
			LoadInitializingTask();
			LoadScene();
		}

		private void LoadScene()
		{
			if (!_isLoadScene)
				return;

			if (_loadSceneAsync.IsDone && (_loadingTask?.IsComplete ?? true) && _loadingTime <= 0)
			{
				_isLoadScene = false;
				_activeScene.Invoke();
				_isLoadInitializingTask = true;
				return;
			}

			_loadingTime -= Time.deltaTime;
		}

		private void LoadInitializingTask()
		{
			if (!_isLoadInitializingTask)
				return;

			if (_initializingTask?.IsComplete ?? true)
			{
				_isLoadInitializingTask = false;
				_onComplete?.Invoke();
				gameObject.SetActive(false);
				return;
			}
		}
	}
}
