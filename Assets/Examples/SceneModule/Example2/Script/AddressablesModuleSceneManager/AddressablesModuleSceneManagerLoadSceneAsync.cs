using System;
using UnityEngine.SceneManagement;

namespace SceneModule.Example2
{
    public class AddressablesModuleSceneManagerLoadSceneAsync : ILoadSceneAsync
    {
        //private variable
        private readonly AddressablesModule.AddressablesModule _addressablesModule;
        
        private string _sceneName;
        private bool _isDone = false;

        //public variable
        public float Progress => _isDone ? 1 : 0;

        //public method
        public AddressablesModuleSceneManagerLoadSceneAsync(AddressablesModule.AddressablesModule addressablesModule) =>
            _addressablesModule = addressablesModule;


        public void Activate() =>
            _addressablesModule.ActivateScene(_sceneName);

        public async void LoadScene(string sceneName, LoadModes loadMode, bool isActivateOnLoad)
        {
            _isDone = false;
            _sceneName = sceneName;
            await _addressablesModule.LoadSceneAsync(sceneName, (LoadSceneMode)loadMode, isActivateOnLoad);
            _isDone = true;
        }
    }
}