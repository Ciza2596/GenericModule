using UnityEngine.SceneManagement;

namespace SceneModule.Implement
{
    public class UnitySceneManager : ISceneManager
    {
        public string CurrentSceneName => SceneManager.GetActiveScene().name;

        public ILoadSceneAsync LoadScene(string sceneName, LoadModes loadMode, bool isActivateOnLoad)
        {
            var loadSceneAsync = SceneManager.LoadSceneAsync(sceneName, (LoadSceneMode)loadMode);
            loadSceneAsync.allowSceneActivation = isActivateOnLoad;
            var unitySceneManagerLoadSceneAsync = new UnitySceneManagerLoadSceneAsync(loadSceneAsync);
            return unitySceneManagerLoadSceneAsync;
        }

        public void UnloadScene(string sceneName) =>
            SceneManager.UnloadSceneAsync(sceneName);
    }
}