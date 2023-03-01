
namespace SceneModule
{
    public interface ISceneManager
    {
        public string CurrentSceneName { get; }

        public ILoadSceneAsync LoadScene(string sceneName, LoadModes loadMode, bool isActivateOnLoad);

        public void UnloadScene(string sceneName);
    }
}
