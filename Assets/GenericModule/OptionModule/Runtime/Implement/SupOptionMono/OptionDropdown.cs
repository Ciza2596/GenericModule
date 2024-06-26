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

        // PreviousIndex, Index
        public event Action<int, int> OnSelect;

        // Index
        public event Action<int> OnConfirm;
        public event Action OnCancel;

        public event Action OnShow;
        public event Action OnHide;

        public string[] Options => _dropdown.Options;

        public bool IsShow => _dropdown.IsShow;

        public int MaxIndex => _dropdown.MaxIndex;

        public int Index => _dropdown.Index;

        public int SelectIndex => _dropdown.SelectIndex;

        public void SetOptions(string[] options) =>
            _dropdown.SetOptions(options);

        public new void Show() =>
            _dropdown.Show();

        public new void Hide() =>
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

        public void Confirm(int index) =>
            _dropdown.Confirm(index);

        public void Confirm() =>
            _dropdown.Confirm();


        public override void Initialize(Option option)
        {
            base.Initialize(option);

            Assert.IsNotNull(_dropdown, $"[OptionDropdown::Initialize] Option: {name} is not set dropdown.");
            Option.OnSetIsUnlock += OnSetIsUnlockImp;

            _dropdown.OnSelect += OnSelectImp;

            _dropdown.OnConfirm += OnConfirmImp;
            _dropdown.OnCancel += OnCancelImp;

            _dropdown.OnShow += OnShowImp;
            _dropdown.OnHide += OnHideImp;
        }

        public override void Release(Option option)
        {
            base.Release(option);

            _dropdown.OnSelect -= OnSelectImp;

            _dropdown.OnConfirm -= OnConfirmImp;
            _dropdown.OnCancel -= OnCancelImp;

            _dropdown.OnShow -= OnShowImp;
            _dropdown.OnHide -= OnHideImp;

            Option.OnSetIsUnlock -= OnSetIsUnlockImp;
        }

        private void OnSelectImp(int previousIndex, int index) =>
            OnSelect?.Invoke(previousIndex, index);

        private void OnConfirmImp(int index) =>
            OnConfirm?.Invoke(index);

        private void OnCancelImp() =>
            OnCancel?.Invoke();

        private void OnShowImp() =>
            OnShow?.Invoke();

        private void OnHideImp() =>
            OnHide?.Invoke();

        private void OnSetIsUnlockImp(string optionKey, bool isUnlock)
        {
            if (optionKey != Option.Key)
                return;

            if (isUnlock)
                _dropdown.EnableInteractable();
            else
                _dropdown.DisableInteractable();
        }
    }
}