using System.Collections.Generic;
using UnityEngine;

namespace AddressablesModule.Example1
{
    [CreateAssetMenu(fileName = "AddressMapListDataOverview", menuName = "AddressableModule/Example1/AddressMapListDataOverview")]
    public class AddressMapListDataOverview : ScriptableObject
    {
        [SerializeField] private List<AddressMap> _addressMapList;

        public AddressMap[] GetAddressMapList() => _addressMapList.ToArray();
    }
    
}