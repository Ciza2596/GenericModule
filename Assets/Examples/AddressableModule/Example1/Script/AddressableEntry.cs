using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AddressableModule.Example1
{
    public class AddressableEntry : MonoBehaviour
    {
        //private variable
        [SerializeField] private bool _isUpdate;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Texture2D _textue2D;
        [Space]
        [SerializeField] private string _address;
        [SerializeField] private Image _image;

        private string _currentAddress;
        
        
        //unity callback
        private async void OnEnable()
        {
            var handle = Addressables.LoadAssetAsync<Object>(_address); 
            await handle.Task.ConfigureAwait(false);
            var texture2D = handle.Result as Texture2D;


            Debug.Log(texture2D != null);
        }
    }
}