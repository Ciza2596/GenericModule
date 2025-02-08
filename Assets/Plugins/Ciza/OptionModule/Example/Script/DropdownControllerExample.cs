using System;
using UnityEngine;

namespace CizaInputModule.Example
{
    public class DropdownControllerExample : MonoBehaviour
    {
        [SerializeField]
        private string[] _options;

        [SerializeField]
        private DropdownController _dropdownController;

        private void Awake()
        {
            _dropdownController.SetOptions(_options);
            _dropdownController.Show();
        }
    }
}