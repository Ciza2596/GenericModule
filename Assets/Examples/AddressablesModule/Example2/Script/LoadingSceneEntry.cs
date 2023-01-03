using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace AddressablesModule.Example2
{
    public class LoadingSceneEntry : MonoBehaviour
    {
        //private variable
        [Space]
        [SerializeField] private string _address = "AddressableModuleExample2_LoadScene";
        
        private AddressablesModule _addressablesModule;


        //unity callback
        private void OnEnable()
        {
            _addressablesModule = new AddressablesModule();
            _addressablesModule.LoadSceneAsync(_address, LoadSceneMode.Additive);
        }
        
        private void OnDisable()
        {
            _addressablesModule.UnloadSceneAsync(_address);
        }
    }
}