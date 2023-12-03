using UnityEngine;

namespace CizaSceneModule
{
	public class TransitionController
	{
		//private variable
		private readonly SceneModule     _sceneModule;
		private          ILoadSceneAsync _loadSceneAsync;

		//public method

		public TransitionController(SceneModule sceneModule, ITransitionControllerConfig transitionControllerConfig)
		{
			_sceneModule   = sceneModule;

			var viewParentPrefab     = transitionControllerConfig.GetViewParentPrefab();
			var viewParentGameObject = Object.Instantiate(viewParentPrefab);
			var viewParentTransform  = viewParentGameObject.transform;

			var isViewName = sceneModule.IsViewName;
			transitionControllerConfig.SetIsViewName(isViewName);

			var transitionInViewName   = sceneModule.TransitionInViewName;
			var transitionInViewPrefab = transitionControllerConfig.GetTransitionInViewPrefab(transitionInViewName);
			var transitionInView       = CreateView<ITransitionView>(viewParentTransform, transitionInViewPrefab);
			var currentSceneName       = sceneModule.CurrentSceneName;
			var releasingTask          = sceneModule.ReleasingTask;

			var loadingViewName   = sceneModule.LoadingViewName;
			var loadingViewPrefab = transitionControllerConfig.GetLoadingViewPrefab(loadingViewName);
			var loadingView       = CreateView<ILoadingView>(viewParentTransform, loadingViewPrefab);
			var nextSceneName     = sceneModule.NextSceneName;
			var loadingTask       = sceneModule.LoadingTask;
			var initializingTask  = sceneModule.InitializingTask;

			var transitionOutViewName   = sceneModule.TransitionOutViewName;
			var transitionOutViewPrefab = transitionControllerConfig.GetTransitionOutPrefab(transitionOutViewName);
			var transitionOutView       = CreateView<ITransitionView>(viewParentTransform, transitionOutViewPrefab);

			transitionInView.Play(() =>
			{
				releasingTask?.Execute();
				UnloadScene(currentSceneName);

				LoadSceneOnBackground(nextSceneName);
				loadingView.Loading(_loadSceneAsync, loadingTask, ActivateScene, initializingTask, () => { transitionOutView.Play(UnloadTransitionScene); });
			});
		}

		//private method
		private void ActivateScene()
		{
			_loadSceneAsync.Activate();
		}

		private void LoadSceneOnBackground(string sceneName) =>
			_loadSceneAsync = _sceneModule.LoadSceneAsync(sceneName, LoadModes.Additive, false);

		private void UnloadScene(string sceneName) =>
			_sceneModule.UnloadScene(sceneName);

		private void UnloadTransitionScene()
		{
			_sceneModule.UnloadTransitionScene();
			_sceneModule.CompleteTask?.Execute();
		}

		private T CreateView<T>(Transform viewParentTransform, GameObject viewPrefab)
		{
			var viewGameObject = Object.Instantiate(viewPrefab, viewParentTransform);
			viewGameObject.SetActive(false);
			var view = viewGameObject.GetComponent<T>();
			return view;
		}
	}
}
