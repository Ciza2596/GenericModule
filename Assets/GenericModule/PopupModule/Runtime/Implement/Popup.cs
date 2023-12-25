using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CizaPopupModule.Implement
{
    public class Popup : MonoBehaviour, IPopup
    {
        [SerializeField]
        private TextSettings _textSettings;

        [SerializeField]
        private ButtonSettings _optionSettings;


        private PopupModule _popupModule;

        public string Key { get; private set; }
        public string DataId { get; private set; }
        public bool HasCancel { get; private set; }

        public string ContentTip { get; private set; }
        public string ConfirmTip { get; private set; }
        public string CancelTip { get; private set; }

        public PopupStates State { get; private set; } = PopupStates.Visible;
        public int Index { get; private set; }
        public bool IsConfirm { get; private set; }

        public GameObject GameObject => gameObject;

        public void Initialize(PopupModule popupModule, string key, string dataId, bool hasCancel, string contentTip, string confirmTip, string cancelTip)
        {
            _popupModule = popupModule;

            Key = key;
            DataId = dataId;
            HasCancel = hasCancel;

            ContentTip = contentTip;
            ConfirmTip = confirmTip;
            CancelTip = cancelTip;

            _optionSettings.ConfirmOption.Button.onClick.AddListener(OnConfirmClick);
            _optionSettings.CancelOption.Button.onClick.AddListener(OnCancelClick);
        }

        public void Release()
        {
            _optionSettings.ConfirmOption.Button.onClick.RemoveListener(OnConfirmClick);
            _optionSettings.CancelOption.Button.onClick.RemoveListener(OnCancelClick);
        }

        public void SetText(string contentText, string confirmText, string cancelText) =>
            _textSettings.SetText(contentText, confirmText, cancelText);

        public UniTask ShowAsync(bool isImmediately)
        {
            return UniTask.CompletedTask;
        }

        public UniTask HideAsync(bool isImmediately)
        {
            return UniTask.CompletedTask;
        }

        public void SetIsConfirm(bool isConfirm) =>
            IsConfirm = isConfirm;

        public void SetIndex(int index) =>
            Index = index;

        public void Confirm() =>
            _optionSettings.ConfirmOption.Confirm();

        public void Cancel() =>
            _optionSettings.CancelOption.Confirm();

        private void OnConfirmClick() =>
            _popupModule.ConfirmAsync(Key);

        private void OnCancelClick() =>
            _popupModule.CancelAsync(Key);


        [Serializable]
        private class TextSettings
        {
            [SerializeField]
            private TMP_Text _contentText;

            [Space]
            [SerializeField]
            private TMP_Text _confirmText;

            [SerializeField]
            private TMP_Text _cancelText;


            public void SetText(string contentText, string confirmText, string cancelText)
            {
                _contentText.text = contentText;
                _confirmText.text = confirmText;
                _cancelText.text = cancelText;
            }
        }

        [Serializable]
        private class ButtonSettings
        {
            [SerializeField]
            private PopupOption _confirmOption;

            [SerializeField]
            private PopupOption _cancelOption;

            public PopupOption ConfirmOption => _confirmOption;

            public PopupOption CancelOption => _cancelOption;
        }
    }
}