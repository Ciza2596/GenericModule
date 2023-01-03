using UnityEngine.Assertions;


namespace SceneModule
{
    public class SceneModule
    {
        //private variable
        private ISceneManager _sceneManager;

        //public variable
        public string TransitionSceneName { get; }

        public string TransitionInViewName { get; private set; }
        public string CurrentSceneName { get; private set; }

        public string LoadingViewName { get; private set; }

        public string TransitionOutViewName { get; private set; }
         public string NextSceneName { get; private set; }


        //public method
        public SceneModule(ISceneModuleConfig sceneModuleConfig, ISceneManager sceneManager)
        {
            TransitionSceneName = sceneModuleConfig.TransitionSceneName;
            Assert.IsTrue(!string.IsNullOrWhiteSpace(TransitionSceneName), "[SceneModule::SceneModule] TransitionSceneName is null.Please check SceneModuleConfig.");
            
            _sceneManager = sceneManager;
        }

        public ILoadSceneAsync LoadSceneAsync(string sceneName, LoadModes loadMode, bool isActivateOnLoad = true) => _sceneManager.LoadScene(sceneName, loadMode, isActivateOnLoad);

        public void UnloadScene(string sceneName) => _sceneManager.UnloadScene(sceneName);


        public void ChangeScene(string transitionInViewName, string currentSceneName, 
            string loadingViewName, string transitionOutViewName, string nextSceneName)
        {
            TransitionInViewName = transitionInViewName;
            CurrentSceneName = currentSceneName;

            LoadingViewName = loadingViewName;

            TransitionOutViewName = transitionOutViewName;
            NextSceneName = nextSceneName;

            LoadSceneAsync(TransitionSceneName, LoadModes.Additive);
        }
    }
}