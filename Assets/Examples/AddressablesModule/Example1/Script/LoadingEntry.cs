using System;
using UnityEngine;
using UnityEngine.UI;

namespace AddressablesModule.Example1
{
    public class LoadingEntry : MonoBehaviour
    {
        //private variable
        
        [SerializeField] private AddressMapListDataOverview _addressMapListDataOverview;
        [SerializeField] private Image[] _images;

        private AddressablesModule _addressablesModule;


        //unity callback

        private void Awake()
        {
            _addressablesModule = new AddressablesModule();
        }

        private async void OnEnable()
        {
            _images[0].sprite = await _addressablesModule.LoadAssetAsync<Sprite>("DUNE_1");


            // var addressMapList = _addressMapListDataOverview.GetAddressMapList();
            //
            // await _addressablesModule.LoadAssetsAsync(addressMapList);
            //
            // var length = addressMapList.Length;
            // for (int i = 0; i < length; i++)
            // {
            //     var addressObjectTypeMap = addressMapList[i];
            //     var address = addressObjectTypeMap.Address;
            //
            //     var sprite = _addressablesModule.GetAsset<Sprite>(address);
            //
            //     var image = _images[i];
            //     image.sprite = sprite;
            // }
        }

        private void OnDisable()
        {
            _addressablesModule.UnloadAsset("DUNE_1", typeof(Sprite));
            //_addressablesModule.UnloadAllAssets();
        }
    }
}