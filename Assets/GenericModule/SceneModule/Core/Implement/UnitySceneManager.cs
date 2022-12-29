using UnityEngine.SceneManagement;

namespace SceneModule.Implement
{
    public class UnitySceneManager : ISceneManager
    {
        public void LoadScene(string sceneName, LoadModes loadMode) =>
            SceneManager.LoadScene(sceneName, new LoadSceneParameters((LoadSceneMode)loadMode));


        public void UnloadScene(string sceneName) =>
            SceneManager.UnloadSceneAsync(sceneName);
    }
}