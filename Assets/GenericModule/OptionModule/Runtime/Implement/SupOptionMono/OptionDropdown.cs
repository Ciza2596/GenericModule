using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaOptionModule.Implement
{
    public class OptionDropdown : OptionSup
    {
        [SerializeField]
        private TMP_Dropdown _dropdown;

        // index
        public event Action<int> OnOptionChanged;

        public int Index => _dropdown.value;

        public void SetIndex(int index) =>
            _dropdown.value = index;

        public void SetOptions(string[] options)
        {
            _dropdown.ClearOptions();
            _dropdown.AddOptions(options.ToList());
        }

        public void Hide() =>
            _dropdown.Hide();

        public override void Initialize(Option option)
        {
            base.Initialize(option);

            Assert.IsNotNull(_dropdown, $"[OptionDropdown::Initialize] Option: {name} is not set dropdown.");
            _dropdown.onValueChanged.AddListener(OnOptionChangedImp);
        }

        public override void Release(Option option)
        {
            base.Release(option);
            _dropdown.onValueChanged.RemoveListener(OnOptionChangedImp);
        }

        private void OnOptionChangedImp(int index) =>
            OnOptionChanged?.Invoke(index);
    }
}