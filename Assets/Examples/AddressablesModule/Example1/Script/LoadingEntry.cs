using UnityEngine;
using UnityEngine.UI;

namespace AddressablesModule.Example1
{
    public class LoadingEntry : MonoBehaviour
    {
        //private variable
        [SerializeField] private AddressMapListDataOverview addressMapListDataOverview;
        [SerializeField] private Image[] _images;

        private AddressablesModule _addressablesModule;


        
        //unity callback
        private async void OnEnable()
        {
            _addressablesModule = new AddressablesModule();

            var addressObjectTypeMapList = addressMapListDataOverview.GetAddressMapList();
            await _addressablesModule.LoadAssetsAsync(addressObjectTypeMapList);

            var length = addressObjectTypeMapList.Length;
            for (int i = 0; i < length; i++)
            {
                var addressObjectTypeMap = addressObjectTypeMapList[i];
                var address = addressObjectTypeMap.Address;

                var sprite = _addressablesModule.GetAsset<Sprite>(address);

                var image = _images[i];
                image.sprite = sprite;
            }
        }
        
        private void OnDisable()
        {
            foreach (var image in _images)
                image.sprite = null;
        }
    }
}