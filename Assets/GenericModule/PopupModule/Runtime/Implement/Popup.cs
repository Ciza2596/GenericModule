using System;
using CizaCore;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CizaPopupModule.Implement
{
    public class Popup : MonoBehaviour, IPopup
    {
        [SerializeField]
        private AnimSettings _animSettings;

        [Space]
        [SerializeField]
        private TextSettings _textSettings;

        [Space]
        [SerializeField]
        private ButtonSettings _optionSettings;

        private Func<string, UniTask> _onConfirmPopupAsync;
        private Func<string, UniTask> _onCancelPopupAsync;

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

        public void Initialize(string key, string dataId, bool hasCancel, string contentTip, string confirmTip, string cancelTip, Func<string, UniTask> onConfirmPopupAsync, Func<string, UniTask> onCancelPopupAsync)
        {
            Key = key;
            DataId = dataId;
            HasCancel = hasCancel;

            ContentTip = contentTip;
            ConfirmTip = confirmTip;
            CancelTip = cancelTip;

            _onConfirmPopupAsync = onConfirmPopupAsync;
            _onCancelPopupAsync = onCancelPopupAsync;

            _optionSettings.ConfirmOption.Button.onClick.AddListener(OnConfirmClick);
            _optionSettings.CancelOption.Button.onClick.AddListener(OnCancelClick);

            gameObject.name = key;

            _optionSettings.ConfirmOption.gameObject.SetActive(true);
            _optionSettings.CancelOption.gameObject.SetActive(HasCancel);
        }

        public void Release()
        {
            _optionSettings.ConfirmOption.Button.onClick.RemoveListener(OnConfirmClick);
            _optionSettings.CancelOption.Button.onClick.RemoveListener(OnCancelClick);
        }

        public void SetText(string contentText, string confirmText, string cancelText) =>
            _textSettings.SetText(contentText, confirmText, cancelText);

        public void SetState(PopupStates state) =>
            State = state;

        public async UniTask ShowAsync(bool isImmediately)
        {
            if (isImmediately)
                _animSettings.PlayShowComplete();

            else
                await _animSettings.PlayShowAsync(default);
        }

        public async UniTask HideAsync(bool isImmediately)
        {
            if (isImmediately)
                _animSettings.PlayHideComplete();

            else
                await _animSettings.PlayHideAsync(default);
        }

        public void SetIsConfirm(bool isConfirm) =>
            IsConfirm = isConfirm;

        public void SetIndex(int index) =>
            Index = index;

        public void Confirm() =>
            _optionSettings.ConfirmOption.Confirm();

        public void Cancel() =>
            _optionSettings.CancelOption.Confirm();

        private async void OnConfirmClick() =>
            await _onConfirmPopupAsync.Invoke(Key);

        private async void OnCancelClick() =>
            await _onCancelPopupAsync.Invoke(Key);


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