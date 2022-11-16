using System;
using System.Collections.Generic;
using UnityEngine;

namespace AddressableModule.Example1
{
    [CreateAssetMenu(fileName = "AddressListDataOverview", menuName = "AddressableModule/Example1/AddressListDataOverview")]
    public class AddressListDataOverview : ScriptableObject
    {
        [SerializeField] private List<string> _addressList;
        [SerializeField] private List<Map> _maps;

        public string[] GetAddressList() => _addressList.ToArray();
        
        [Serializable]
        public class Map
        {
            public string Address;
            public Type Type;
        }
    }
    
}