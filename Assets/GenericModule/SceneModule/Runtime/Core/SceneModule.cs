using UnityEngine.Assertions;


namespace SceneModule
{
    public class SceneModule
    {
        //private variable
        private readonly ISceneManager _sceneManager;

        //public variable
        public string TransitionSceneName { get; }

        public string TransitionInViewName { get; private set; }
        public string CurrentSceneName { get; private set; }
        public ISceneTask ReleasingTask { get; private set; }


        public string LoadingViewName { get; private set; }

        public string TransitionOutViewName { get; private set; }
        public string NextSceneName { get; private set; }
        public ISceneTask LoadingTask { get; private set; }


        //public method
        public SceneModule(ISceneModuleConfig sceneModuleConfig, ISceneManager sceneManager)
        {
            TransitionSceneName = sceneModuleConfig.TransitionSceneName;
            Assert.IsTrue(!string.IsNullOrWhiteSpace(TransitionSceneName),
                "[SceneModule::SceneModule] TransitionSceneName is null.Please check SceneModuleConfig.");

            _sceneManager = sceneManager;
        }

        public ILoadSceneAsync LoadSceneAsync(string sceneName, LoadModes loadMode, bool isActivateOnLoad = true,
            ISceneTask sceneTask = null)
        {
            sceneTask?.Load();

            var loadSceneAsync = _sceneManager.LoadScene(sceneName, loadMode, isActivateOnLoad);
            return loadSceneAsync;
        }

        public void UnloadScene(string sceneName,
            ISceneTask sceneTask = null)
        {
            sceneTask?.Release();

            _sceneManager.UnloadScene(sceneName);
        }


        public void ChangeScene(string transitionInViewName, string currentSceneName,
            string loadingViewName, string transitionOutViewName, string nextSceneName, ISceneTask releasingTask = null,
            ISceneTask loadingTask = null)
        {
            TransitionInViewName = transitionInViewName;
            CurrentSceneName = currentSceneName;
            ReleasingTask = releasingTask;

            LoadingViewName = loadingViewName;

            TransitionOutViewName = transitionOutViewName;
            NextSceneName = nextSceneName;
            LoadingTask = LoadingTask;

            LoadSceneAsync(TransitionSceneName, LoadModes.Additive);
        }
    }
}