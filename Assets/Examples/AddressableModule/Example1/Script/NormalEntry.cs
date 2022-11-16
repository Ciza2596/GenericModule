using UnityEngine;
using UnityEngine.UI;

namespace AddressableModule.Example1
{
    public class NormalEntry : MonoBehaviour
    {
        //private variable
        [SerializeField]
        private AddressListDataOverview _addressListDataOverview;
        
        [SerializeField] private Image[] _images;
        
        private AddressableModule _addressableModule;

        
        //unity callback
        private async void OnEnable()
        {
            _addressableModule = new AddressableModule();
            
            var addressList = _addressListDataOverview.GetAddressList();
            
            var length = addressList.Length;
            for (int i = 0; i < length; i++)
            {
                var address = addressList[i];
                var sprite = await _addressableModule.GetAssetAsync<Sprite>(address);
                
                var image = _images[i];
                image.sprite = sprite;
            }
        }


        private void OnDisable()
        {
            foreach (var image in _images)
                image.sprite = null;
            
            _addressableModule.ReleaseAllAsset();
        }
    }
}