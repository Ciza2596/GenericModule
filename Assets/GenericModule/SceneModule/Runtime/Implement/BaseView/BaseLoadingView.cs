using System;
using UnityEngine;

namespace CizaSceneModule.Implement
{
	public class BaseLoadingView : MonoBehaviour, ILoadingView
	{
		//private variable
		[SerializeField]
		private float _defaultLoadingTime = 1;

		[SerializeField]
		private float _defaultWaitingAddInitializingTaskTime = 0.05f;

		private ILoadSceneAsync   _loadSceneAsync;
		private ILoadingTask      _loadingTask;
		private Action            _activeScene;
		private IInitializingTask _initializingTask;
		private Action            _onComplete;

		private bool _isInitialized;

		private float _loadingTime;
		private bool  _isLoadScene;

		private float _waitingAddInitializingTaskTime;
		private bool  _isLoadInitializingTask;

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
		private void Update()
		{
			if (!_isInitialized)
				return;

			LoadInitializingTask();
			WaitingAddInitializingTask();
			LoadScene();
		}

		private void LoadScene()
		{
			if (!_isLoadScene)
				return;

			var isComplete = _loadingTask != null ? _loadingTask.IsComplete : true;
			if (_loadSceneAsync.IsDone && isComplete && _loadingTime < 0)
			{
				_activeScene.Invoke();
				_waitingAddInitializingTaskTime = _defaultWaitingAddInitializingTaskTime;
				_isLoadScene                    = false;
				return;
			}

			_loadingTime -= Time.deltaTime;
		}

		private void WaitingAddInitializingTask()
		{
			if (_isLoadScene || _isLoadInitializingTask)
				return;

			if (_waitingAddInitializingTaskTime < 0)
			{
				_initializingTask?.Execute();
				_isLoadInitializingTask = true;
				return;
			}

			_waitingAddInitializingTaskTime -= Time.deltaTime;
		}

		private void LoadInitializingTask()
		{
			if (!_isLoadInitializingTask)
				return;

			var isComplete = _initializingTask != null ? _initializingTask.IsComplete : true;
			if (isComplete)
			{
				_isLoadInitializingTask = false;
				_onComplete?.Invoke();
				gameObject.SetActive(false);
				return;
			}
		}
	}
}
