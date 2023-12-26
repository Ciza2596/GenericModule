using System;
using CizaCore;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaPopupModule.Implement
{
    public class Popup : MonoBehaviour, IPopup
    {
        [SerializeField]
        private AnimSettings _animSettings;

        [Space]
        [SerializeField]
        private Settings _settings;

        private Func<string, UniTask> _onConfirmPopupAsync;
        private Func<string, UniTask> _onCancelPopupAsync;

        public string Key { get; private set; }
        public string DataId { get; private set; }

        public bool IsAutoHide { get; private set; }
        public bool HasCancel { get; private set; }

        public string ContentTip { get; private set; }
        public string ConfirmTip { get; private set; }
        public string CancelTip { get; private set; }

        public PopupStates State { get; private set; } = PopupStates.Visible;
        public int Index { get; private set; }
        public bool IsConfirm { get; private set; }

        public GameObject GameObject => gameObject;

        public void Initialize(string key, string dataId, bool isAutoHide, bool hasCancel, string contentTip, string confirmTip, string cancelTip, Action<string, int> onSelect, Func<string, UniTask> onConfirmPopupAsync, Func<string, UniTask> onCancelPopupAsync)
        {
            Key = key;
            DataId = dataId;

            IsAutoHide = isAutoHide;
            HasCancel = hasCancel;

            ContentTip = contentTip;
            ConfirmTip = confirmTip;
            CancelTip = cancelTip;

            _onConfirmPopupAsync = onConfirmPopupAsync;
            _onCancelPopupAsync = onCancelPopupAsync;

            _settings.Initialize();

            _settings.ConfirmOption.Initialize(Key, PopupModule.ConfrimIndex, onSelect);
            _settings.CancelOption.Initialize(Key, PopupModule.CancelIndex, onSelect);

            _settings.ConfirmOption.Button.onClick.AddListener(OnConfirmClick);
            _settings.CancelOption.Button.onClick.AddListener(OnCancelClick);

            gameObject.name = key;

            _settings.ConfirmOption.gameObject.SetActive(true);
            _settings.CancelOption.gameObject.SetActive(HasCancel);
        }

        public void Release()
        {
            _settings.ConfirmOption.Button.onClick.RemoveListener(OnConfirmClick);
            _settings.CancelOption.Button.onClick.RemoveListener(OnCancelClick);

            _settings.Release();
        }

        public void SetText(string contentText, string confirmText, string cancelText) =>
            _settings.SetText(contentText, confirmText, cancelText);

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

        public void Select(int index)
        {
            GetPopupOption(Index).Unselect();

            Index = index;
            GetPopupOption(Index).Select();
        }

        public void Confirm() =>
            _settings.ConfirmOption.Confirm();

        public void Cancel() =>
            _settings.CancelOption.Confirm();

        private async void OnConfirmClick() =>
            await _onConfirmPopupAsync.Invoke(Key);

        private async void OnCancelClick() =>
            await _onCancelPopupAsync.Invoke(Key);


        private PopupOption GetPopupOption(int index)
        {
            if (index == PopupModule.ConfrimIndex)
                return _settings.ConfirmOption;

            return _settings.CancelOption;
        }


        [Serializable]
        private class Settings
        {
            [SerializeField]
            private TMP_Text _contentText;

            [Space]
            [SerializeField]
            private Transform _optionParent;

            [SerializeField]
            private GameObject _template;

            [SerializeField]
            private GameObject _optionPrefab;

            public PopupOption ConfirmOption { get; private set; }
            public PopupOption CancelOption { get; private set; }

            public void Initialize()
            {
                _template.SetActive(false);

                ConfirmOption = Instantiate(_optionPrefab, _optionParent).GetComponent<PopupOption>();
                CancelOption = Instantiate(_optionPrefab, _optionParent).GetComponent<PopupOption>();
            }

            public void Release()
            {
                DestroyOrImmediate(ConfirmOption.gameObject);
                DestroyOrImmediate(CancelOption.gameObject);
            }

            public void SetText(string contentText, string confirmText, string cancelText)
            {
                _contentText.text = contentText;
                ConfirmOption.SetText(confirmText);
                CancelOption.SetText(cancelText);
            }

            private void DestroyOrImmediate(Object obj)
            {
                if (Application.isPlaying)
                    Destroy(obj);
                else
                    DestroyImmediate(obj);
            }
        }
    }
}