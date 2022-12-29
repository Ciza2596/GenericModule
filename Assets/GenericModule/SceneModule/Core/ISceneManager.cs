
namespace SceneModule
{
    public interface ISceneManager
    {
        public void LoadScene(string sceneName, LoadModes loadMode);

        public void UnloadScene(string sceneName);
    }
}
