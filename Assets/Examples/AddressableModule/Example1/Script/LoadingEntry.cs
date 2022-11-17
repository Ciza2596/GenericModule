using UnityEngine;
using UnityEngine.UI;

namespace AddressableModule.Example1
{
    public class LoadingEntry : MonoBehaviour
    {
        //private variable
        [SerializeField]
        private AddressMapListDataOverview addressMapListDataOverview;

        [SerializeField] private Image[] _images;

        private AddressableModule _addressableModule;
        

        //unity callback
        private async void OnEnable()
        {
            _addressableModule = new AddressableModule();
            
            var addressObjectTypeMapList = addressMapListDataOverview.GetAddressMapList();
            await _addressableModule.LoadAssetsAsync(addressObjectTypeMapList);

            var length = addressObjectTypeMapList.Length;
            for (int i = 0; i < length; i++)
            {
                var addressObjectTypeMap = addressObjectTypeMapList[i];
                var address = addressObjectTypeMap.Address;
                
                var sprite = _addressableModule.GetAsset<Sprite>(address);
                
                var image = _images[i];
                image.sprite = sprite;
            }
        }


        private void OnDisable()
        {
            // foreach (var image in _images)
            //     image.sprite = null;

            _addressableModule.ReleaseAllAsset();
        }
    }
}
