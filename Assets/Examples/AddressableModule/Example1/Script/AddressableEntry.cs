using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace AddressablesModule.Example1
{
    public class AddressableEntry : MonoBehaviour
    {
        //private variable
        [Space]
        [SerializeField] private string _address = "DUNE_1";
        [SerializeField] private Image _image;

        private AsyncOperationHandle<Sprite> _asyncOperationHandle;
        private Sprite _sprite;
        


        //unity callback
        private async void OnEnable()
        {
            _asyncOperationHandle = Addressables.LoadAssetAsync<Sprite>(_address);
            _sprite = await _asyncOperationHandle.Task.ConfigureAwait(false);
            
            _image.sprite = _sprite;
        }
        
        private void OnDisable()
        {
            _image.sprite = null;
            Addressables.Release(_asyncOperationHandle);
        }
    }
}