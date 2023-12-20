using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace CizaOptionModule.Implement
{
    public class OptionDropdown : MonoBehaviour
    {
        [SerializeField]
        private Dropdown _dropdown;

        public Dropdown Dropdown
        {
            get
            {
                Assert.IsNotNull(_dropdown, $"[OptionDropdown::Dropdown] Option: {name} is not set dropdown.");
                return _dropdown;
            }
        }
    }
}