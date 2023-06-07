using UnityEngine;
using UnityEngine.SceneManagement;

namespace CizaAddressablesModule.Example2
{
    public class LoadingSceneEntry : MonoBehaviour
    {
        //private variable
        [Space]
        [SerializeField] private string _address = "AddressableModuleExample2_LoadScene";
        
        private AddressablesModule _addressablesModule;


        private void Awake()
        {
            _addressablesModule = new AddressablesModule();
        }

        //unity callback
        private async void OnEnable()
        {
            await _addressablesModule.LoadSceneAsync(_address, LoadSceneMode.Additive, false);
            _addressablesModule.ActivateScene(_address);
        }
        
        private void OnDisable()
        {
            _addressablesModule.UnloadSceneAsync(_address);
        }
    }
}