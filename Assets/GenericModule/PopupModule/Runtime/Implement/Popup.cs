using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CizaPopupModule.Implement
{
    public class Popup : MonoBehaviour, IPopup
    {
        [SerializeField]
        private TextSettings _textSettings;

        [SerializeField]
        private ButtonSettings _buttonSettings;


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

        public void Initialize(string key, string dataId, bool hasCancel, string contentTip, string confirmTip, string cancelTip)
        {
            Key = key;
            DataId = dataId;
            HasCancel = hasCancel;

            ContentTip = contentTip;
            ConfirmTip = confirmTip;
            CancelTip = cancelTip;
        }

        public void Release() { }

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

        public void Select(int index) =>
            Index = index;

        public void Confirm(int index)
        {
            throw new NotImplementedException();
        }

        public void Confirm()
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        [Serializable]
        private class TextSettings
        {
            [SerializeField]
            private TMP_Text _contentText;

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
            private Button _confirmButton;

            [SerializeField]
            private Button _cancelButton;

            public Button ConfirmButton => _confirmButton;
            
            public Button CancelButton => _cancelButton;
        }
    }
}