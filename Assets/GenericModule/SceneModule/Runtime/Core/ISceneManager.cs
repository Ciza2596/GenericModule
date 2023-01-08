
namespace SceneModule
{
    public interface ISceneManager
    {
        public ILoadSceneAsync LoadScene(string sceneName, LoadModes loadMode, bool isActivateOnLoad);

        public void UnloadScene(string sceneName);
    }
}
