using System;
using System.Linq;
using Sirenix.OdinInspector;
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

        public void Show()
        {
            _dropdown.Show();
        }

        [Button]
        public void Hide()
        {
            _dropdown.Hide();
            //_dropdown.is
        }

        [Button]
        public void Confirm() { }

        private void Awake()
        {
            _dropdown.onValueChanged.AddListener(OnValueChangedImp);
        }

        private void Update()
        {
            Debug.LogWarning(_dropdown.value);
        }

        private void OnValueChangedImp(int index)
        {
            OnValueChanged?.Invoke(index);
            Debug.LogWarning("Change");
        }
    }
}