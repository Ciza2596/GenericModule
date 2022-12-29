using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneModule
{
    public class TransitionController
    {
        //private variable
        private SceneModule _sceneModule;
        private AsyncOperation _loadSceneAsync;
        
        //public variable
        public bool IsDone => _loadSceneAsync.isDone;
        

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


            var loadingViewName = sceneModule.LoadingViewName;
            var loadingViewPrefab = transitionControllerConfig.GetLoadingViewPrefab(loadingViewName);
            var loadingView = CreateView<ILoadingView>(viewParentTransform, loadingViewPrefab);
            var nextSceneName = sceneModule.NextSceneName;


            var transitionOutViewName = sceneModule.TransitionOutViewName;
            var transitionOutViewPrefab = transitionControllerConfig.GetTransitionOutPrefab(transitionOutViewName);
            var transitionOutView = CreateView<ITransitionView>(viewParentTransform, transitionOutViewPrefab);
            var transitionSceneName = sceneModule.TransitionSceneName;


            transitionInView.Play(() =>
            {
                UnloadScene(currentSceneName);

                LoadSceneOnBackground(nextSceneName);
                loadingView.Loading(this,
                    () =>
                    {
                        ShowScene();
                        transitionOutView.Play(() => UnloadScene(transitionSceneName));
                    });
            });
        }

        //private method
        private void ShowScene()
        {
            _loadSceneAsync.allowSceneActivation = true;
        }

        private void LoadSceneOnBackground(string sceneName)
        {
            _loadSceneAsync = SceneManager.LoadSceneAsync(sceneName);
            _loadSceneAsync.allowSceneActivation = false;
        }

        private void UnloadScene(string sceneName)
        {
            var unloadSceneAsync = SceneManager.UnloadSceneAsync(sceneName);
            unloadSceneAsync.allowSceneActivation = true;
        }

        private T CreateView<T>(Transform viewParentTransform, GameObject viewPrefab)
        {
            var viewGameObject = Object.Instantiate(viewPrefab, viewParentTransform);
            var view = viewGameObject.GetComponent<T>();
            return view;
        }
    }
}