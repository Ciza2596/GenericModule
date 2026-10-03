using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAddressablesModule.Editor
{
    [Serializable]
    public class AddAddressableDataList
    {
        [SerializeField] 
        protected AddAddressableData[] _addAddressableDatas = Array.Empty<AddAddressableData>();
        
        public AddAddressableData[] AddAddressableDatas => _addAddressableDatas;
        
        [Preserve]
        public AddAddressableDataList(){}
    }
}
