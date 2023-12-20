using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace CizaInputModule.Example
{
    public class DropdownController : MonoBehaviour
    {
        [SerializeField]
        private TMP_Dropdown _dropdown;

        public event Action<int> OnValueChanged;
        
        public void SetOptions(string[] options)
        {
            _dropdown.ClearOptions();
            _dropdown.AddOptions(options.ToList());
        }

        public void Hide()
        {
            _dropdown.Hide();
        }

        private void Awake()
        {
            _dropdown.onValueChanged.AddListener(OnValueChangedImp);
        }

        private void OnValueChangedImp(int index)
        {
            OnValueChanged?.Invoke(index);
        }
    }
}