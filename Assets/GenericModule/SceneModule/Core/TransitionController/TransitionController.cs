namespace SceneModule
{
    public class TransitionController
    {
        //public method

        public TransitionController(ISceneModuleReadModel sceneModuleReadModel,
            ITransitionControllerConfig transitionControllerConfig)
        {
            var transitionInViewName = sceneModuleReadModel.TransitionInViewName;
            var transitionInView = transitionControllerConfig.GetTransitionInView(transitionInViewName);
            var currentSceneName = sceneModuleReadModel.CurrentSceneName;


            var loadingViewName = sceneModuleReadModel.LoadingViewName;
            var loadingView = transitionControllerConfig.GetLoadingView(loadingViewName);
            var nextSceneName = sceneModuleReadModel.NextSceneName;


            var transitionOutViewName = sceneModuleReadModel.TransitionOutViewName;
            var transitionOutView = transitionControllerConfig.GetTransitionOutView(transitionOutViewName);
            var transitionSceneName = sceneModuleReadModel.TransitionSceneName;


            transitionInView.Play(() =>
            {
                UnloadScene(currentSceneName);

                loadingView.Loading(() => LoadSceneOnBackGround(nextSceneName),
                    () =>
                    {
                        ShowScene(nextSceneName);
                        transitionOutView.Play(() => UnloadScene(transitionSceneName));
                    });
            });
        }

        //private method
        private void ShowScene(string sceneName)
        {
        }

        private void LoadSceneOnBackGround(string sceneName)
        {
        }

        private void UnloadScene(string sceneName)
        {
        }
    }
}