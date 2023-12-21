using System;
using CizaCore.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaOptionModule.Implement
{
    public class OptionDropdown : OptionSup
    {
        [SerializeField]
        private Dropdown _dropdown;

        // index
        public event Action<int> OnOptionChanged;

        public bool IsShow => _dropdown.IsShow;

        public int Index => _dropdown.Index;

        public void SetIndex(int index) =>
            _dropdown.Confirm(index);

        public void SetOptions(string[] options) =>
            _dropdown.SetOptions(options);

        public void Show() =>
            _dropdown.Show();

        public void Hide() =>
            _dropdown.Hide();

        public void MoveToUp()
        {
            if (!_dropdown.IsShow)
                return;

            _dropdown.Select(_dropdown.SelectIndex - 1, false);
        }

        public void MoveToDown()
        {
            if (!_dropdown.IsShow)
                return;

            _dropdown.Select(_dropdown.SelectIndex + 1, false);
        }

        public void Confirm()
        {
            if (!_dropdown.IsShow)
                return;

            _dropdown.Confirm();
        }

        public override void Initialize(Option option)
        {
            base.Initialize(option);

            Assert.IsNotNull(_dropdown, $"[OptionDropdown::Initialize] Option: {name} is not set dropdown.");
            _dropdown.OnIndexChanged += OnOptionChangedImp;
        }

        public override void Release(Option option)
        {
            base.Release(option);
            _dropdown.OnIndexChanged -= OnOptionChangedImp;
        }

        private void OnOptionChangedImp(int index) =>
            OnOptionChanged?.Invoke(index);
    }
}