using UnityEngine;
using UnityEngine.UI;

namespace CizaAddressablesModule.Example1
{
    public class AddressableModuleExample1ForTestBuild : MonoBehaviour
    {
        [SerializeField] private string _address = "DUNE_1";
        [Space] [SerializeField] private AddressMapListDataOverview _addressMapListDataOverview;
        [SerializeField] private Image _image;

        [Space] [SerializeField] private Button _loadButton;
        [SerializeField] private Button _unloadButton;

        private AddressablesModule _addressablesModule;


        //unity callback

        private void Awake()
        {
            _addressablesModule = new AddressablesModule();

            _loadButton.onClick.AddListener(OnLoadButtonClick);
            _unloadButton.onClick.AddListener(OnUnloadButtonClick);
        }

        //private method
        private async void OnLoadButtonClick() =>
            _image.sprite = await _addressablesModule.LoadAssetAsync<Sprite>(_address);


        private void OnUnloadButtonClick() =>
            _addressablesModule.UnloadAsset(_address, typeof(Sprite));
    }
}