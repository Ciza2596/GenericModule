using UnityEngine.SceneManagement;

namespace SceneModule.Example2
{
    public class AddressablesModuleSceneManagerLoadSceneAsync : ILoadSceneAsync
    {
        //private variable
        private readonly string _sceneName;
        private readonly AddressablesModule.AddressablesModule _addressablesModule;
        private bool _isDone = false;

        //public variable
        public float Progress => _isDone ? 0 : 1;

        //public method
        public AddressablesModuleSceneManagerLoadSceneAsync(AddressablesModule.AddressablesModule addressablesModule,
            string sceneName, LoadModes loadMode, bool isActivateOnLoad)
        {
            _sceneName = sceneName;
            _addressablesModule = addressablesModule;
            
            LoadScene(sceneName, loadMode, isActivateOnLoad);
        }

        public void Activate() =>
            _addressablesModule.ActivateScene(_sceneName);

        private async void LoadScene(string sceneName, LoadModes loadMode, bool isActivateOnLoad)
        {
            _isDone = false;
            await _addressablesModule.LoadSceneAsync(sceneName, (LoadSceneMode)loadMode, isActivateOnLoad);
            _isDone = true;
        }
    }
}