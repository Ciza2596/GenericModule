
using System;
using UnityEngine.SceneManagement;

namespace SceneModule.Implement
{
    public class AddressablesModuleSceneManager : ISceneManager
    {
        //private variable
        private AddressablesModule.AddressablesModule _addressablesModule;

        //public method
        public AddressablesModuleSceneManager(AddressablesModule.AddressablesModule addressablesesModule)
            => _addressablesModule = addressablesesModule;


        public ILoadSceneAsync LoadScene(string sceneName, LoadModes loadMode, bool isActivateOnLoad)
        {
            var addressablesModuleSceneManagerLoadSceneAsync = new AddressablesModuleSceneManagerLoadSceneAsync(_addressablesModule);
            addressablesModuleSceneManagerLoadSceneAsync.LoadScene(sceneName, loadMode, isActivateOnLoad);
            
            return addressablesModuleSceneManagerLoadSceneAsync;
        }

        public async void UnloadScene(string sceneName)
        {
            try
            {
                await _addressablesModule.UnloadSceneAsync(sceneName);
            }
            catch (Exception e)
            {
                SceneManager.UnloadSceneAsync(sceneName);
            } 
        }
    }
}