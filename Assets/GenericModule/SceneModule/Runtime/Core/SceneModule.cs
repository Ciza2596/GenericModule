using UnityEngine.Assertions;

namespace CizaSceneModule
{
	public class SceneModule
	{
		//private variable
		private readonly ISceneManager _sceneManager;

		//public variable
		public string TransitionSceneName { get; }

		public bool IsViewName { get; private set; }

		public string TransitionInViewName { get; private set; }

		public string         CurrentSceneName { get; private set; }
		public IReleasingTask ReleasingTask    { get; private set; }

		public string LoadingViewName { get; private set; }

		public string            TransitionOutViewName { get; private set; }
		public string            NextSceneName         { get; private set; }
		public ILoadingTask      LoadingTask           { get; private set; }
		public IInitializingTask InitializingTask      { get; private set; }

		//public method
		public SceneModule(ISceneModuleConfig sceneModuleConfig, ISceneManager sceneManager)
		{
			TransitionSceneName = sceneModuleConfig.TransitionSceneName;
			Assert.IsTrue(!string.IsNullOrWhiteSpace(TransitionSceneName), "[SceneModule::SceneModule] TransitionSceneName is null.Please check SceneModuleConfig.");

			_sceneManager = sceneManager;
		}

		public ILoadSceneAsync LoadSceneAsync(string sceneName, LoadModes loadMode, bool isActivateOnLoad = true) =>
			_sceneManager.LoadScene(sceneName, loadMode, isActivateOnLoad);

		public void UnloadScene(string sceneName) =>
			_sceneManager.UnloadScene(sceneName);

		public void ChangeScene(string transitionInViewName, string loadingViewName, string transitionOutViewName, string nextSceneName, IReleasingTask releasingTask = null, ILoadingTask loadingTask = null, IInitializingTask initializingTask = null) =>
			ChangeScene(transitionInViewName, null, loadingViewName, transitionOutViewName, nextSceneName, releasingTask, loadingTask, initializingTask);

		public void ChangeScene(string transitionInViewName, string currentSceneName, string loadingViewName, string transitionOutViewName, string nextSceneName, IReleasingTask releasingTask = null, ILoadingTask loadingTask = null, IInitializingTask initializingTask = null) =>
			ChangeScene(true, transitionInViewName, currentSceneName, loadingViewName, transitionOutViewName, nextSceneName, releasingTask, loadingTask, initializingTask);

		public void ChangeScene(string transitionViewTag, string nextSceneName, IReleasingTask releasingTask = null, ILoadingTask loadingTask = null, IInitializingTask initializingTask = null) =>
			ChangeScene(transitionViewTag, null, nextSceneName, releasingTask, loadingTask, initializingTask);

		public void ChangeScene(string transitionViewTag, string currentSceneName, string nextSceneName, IReleasingTask releasingTask = null, ILoadingTask loadingTask = null, IInitializingTask initializingTask = null) =>
			ChangeScene(false, transitionViewTag, currentSceneName, transitionViewTag, transitionViewTag, nextSceneName, releasingTask, loadingTask, initializingTask);

		private void ChangeScene(bool isViewName, string transitionInViewName, string currentSceneName, string loadingViewName, string transitionOutViewName, string nextSceneName, IReleasingTask releasingTask, ILoadingTask loadingTask, IInitializingTask initializingTask)
		{
			IsViewName = isViewName;

			TransitionInViewName = transitionInViewName;
			CurrentSceneName     = string.IsNullOrWhiteSpace(currentSceneName) ? _sceneManager.CurrentSceneName : currentSceneName;
			ReleasingTask        = releasingTask;

			LoadingViewName = loadingViewName;

			TransitionOutViewName = transitionOutViewName;
			NextSceneName         = nextSceneName;
			LoadingTask           = loadingTask;

			InitializingTask = initializingTask;

			LoadSceneAsync(TransitionSceneName, LoadModes.Additive);
		}
	}
}
