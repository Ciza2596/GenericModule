using UnityEngine.SceneManagement;


namespace SceneModule
{
    
    public class SceneModule
    {
        //public variable
        public string TransitionSceneName { get; }
        
        public string CurrentSceneName { get; private set; }
        public string TransitionInViewName { get; private set; }
        
        public string LoadingViewName { get; private set; }

        public string NextSceneName { get; private set; }
        public string TransitionOutViewName { get; private set; }
        
        
        //public method
        public SceneModule(SceneModuleConfig sceneModuleConfig) =>
            TransitionSceneName = sceneModuleConfig.TransitionSceneName;
        

        public void LoadScene(string sceneName, LoadModes loadMode) =>
            SceneManager.LoadScene(sceneName, new LoadSceneParameters((LoadSceneMode)loadMode));

        
        public void UnloadScene(string sceneName) => SceneManager.UnloadSceneAsync(sceneName);

        

        public void ChangeScene(string unloadSceneName, string transitionInViewName,
            string loadingViewName, string loadSceneName, string transitionOutViewName)
        {
            CurrentSceneName = unloadSceneName;
            TransitionInViewName = transitionInViewName;

            LoadingViewName = loadingViewName;

            NextSceneName = loadSceneName;
            TransitionOutViewName = transitionOutViewName;

            LoadScene(TransitionSceneName, LoadModes.Additive);
        }
        
    }
}
