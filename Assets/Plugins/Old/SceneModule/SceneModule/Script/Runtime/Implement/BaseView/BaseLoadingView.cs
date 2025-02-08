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

		private bool _isInitialized;

		private float _loadingTime;
		private bool  _isLoadScene;

		private bool _isCheckHasInitializingTask;

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

			_isInitialized = true;
		}

		//unity callback
		private void FixedUpdate()
		{
			if (!_isInitialized)
				return;

			LoadInitializingTask();
			CheckHasInitializingTask();
			LoadScene(Time.fixedTime);
		}

		private void LoadScene(float deltaTime)
		{
			if (!_isLoadScene)
				return;

			var isComplete = _loadingTask != null ? _loadingTask.IsComplete : true;
			if (_loadSceneAsync.IsDone && isComplete && _loadingTime < 0)
			{
				_isLoadScene                  = false;
				_isCheckHasInitializingTask = true;
				_activeScene.Invoke();
				return;
			}

			_loadingTime -= deltaTime;
		}

		private void CheckHasInitializingTask()
		{
			if (!_isCheckHasInitializingTask)
				return;

			var hasInitializingTask = _initializingTask != null ? _initializingTask.HasTask : true;
			if (!hasInitializingTask)
				return;

			_isCheckHasInitializingTask = false;
			_initializingTask?.Execute();
			_isLoadInitializingTask       = true;
		}

		private void LoadInitializingTask()
		{
			if (!_isLoadInitializingTask)
				return;

			var isComplete = _initializingTask != null ? _initializingTask.IsComplete : true;
			if (!isComplete)
				return;

			_isInitialized          = false;
			_isLoadInitializingTask = false;
			_onComplete?.Invoke();
			gameObject.SetActive(false);
			return;
		}
	}
}
