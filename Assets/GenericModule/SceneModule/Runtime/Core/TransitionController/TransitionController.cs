using UnityEngine;

namespace SceneModule
{
    public class TransitionController
    {
        //private variable
        private readonly SceneModule _sceneModule;
        private ILoadSceneAsync _loadSceneAsync;

        //public method

        public TransitionController(SceneModule sceneModule,
            ITransitionControllerConfig transitionControllerConfig)
        {
            _sceneModule = sceneModule;

            var viewParentPrefab = transitionControllerConfig.GetViewParentPrefab();
            var viewParentGameObject = Object.Instantiate(viewParentPrefab);
            var viewParentTransform = viewParentGameObject.transform;

            var transitionInViewName = sceneModule.TransitionInViewName;
            var transitionInViewPrefab = transitionControllerConfig.GetTransitionInViewPrefab(transitionInViewName);
            var transitionInView = CreateView<ITransitionView>(viewParentTransform, transitionInViewPrefab);
            var currentSceneName = sceneModule.CurrentSceneName;
            var releasingTask = sceneModule.ReleasingTask;


            var loadingViewName = sceneModule.LoadingViewName;
            var loadingViewPrefab = transitionControllerConfig.GetLoadingViewPrefab(loadingViewName);
            var loadingView = CreateView<ILoadingView>(viewParentTransform, loadingViewPrefab);
            var nextSceneName = sceneModule.NextSceneName;
            var loadingTask = sceneModule.LoadingTask;


            var transitionOutViewName = sceneModule.TransitionOutViewName;
            var transitionOutViewPrefab = transitionControllerConfig.GetTransitionOutPrefab(transitionOutViewName);
            var transitionOutView = CreateView<ITransitionView>(viewParentTransform, transitionOutViewPrefab);
            var transitionSceneName = sceneModule.TransitionSceneName;


            transitionInView.Play(() =>
            {
                UnloadScene(currentSceneName);
                releasingTask?.Execute();

                LoadSceneOnBackground(nextSceneName);
                loadingView.Loading(_loadSceneAsync, loadingTask,
                    () =>
                    {
                        ActivateScene();
                        transitionOutView.Play(() => UnloadScene(transitionSceneName));
                    });
            });
        }

        //private method
        private void ActivateScene() =>
            _loadSceneAsync.Activate();


        private void LoadSceneOnBackground(string sceneName) =>
            _loadSceneAsync = _sceneModule.LoadSceneAsync(sceneName, LoadModes.Additive, false);


        private void UnloadScene(string sceneName) =>
            _sceneModule.UnloadScene(sceneName);


        private T CreateView<T>(Transform viewParentTransform, GameObject viewPrefab)
        {
            var viewGameObject = Object.Instantiate(viewPrefab, viewParentTransform);
            viewGameObject.SetActive(false);
            var view = viewGameObject.GetComponent<T>();
            return view;
        }
    }
}