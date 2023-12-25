using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaPopupModule
{
    public class PopupModule
    {
        public const int ConfrimIndex = 0;
        public const int CancelIndex = 1;

        private readonly IPopupModuleConfig _popupModuleConfig;

        private readonly Dictionary<string, IPopup> _popupMapByKey = new Dictionary<string, IPopup>();

        private Transform _root;

        // key
        public event Action<string> OnShowingStart;

        // key
        public event Action<string> OnShowingComplete;

        // Key
        public event Action<string> OnHidingStart;

        // Key
        public event Action<string> OnHidingComplete;

        // Key
        public event Action<string> OnConfirm;

        // Key
        public event Action<string> OnCancel;

        // Tip
        public event Func<string, string> OnTranslate;

        public bool IsInitialized { get; private set; }


        public PopupModule(IPopupModuleConfig popupModuleConfig) =>
            _popupModuleConfig = popupModuleConfig;

        public void Initialize(Transform parent = null)
        {
            if (IsInitialized)
                return;

            IsInitialized = true;

            var rootGameObject = Object.Instantiate(_popupModuleConfig.CanvasPrefab);
            rootGameObject.name = _popupModuleConfig.RootName;
            _root = rootGameObject.transform;

            if (parent != null)
                _root.SetParent(parent);

            else if (_popupModuleConfig.IsDontDestroyOnLoad)
                Object.DontDestroyOnLoad(_root.gameObject);
        }

        public void Release()
        {
            if (!IsInitialized)
                return;

            DestroyAllPopups();

            var root = _root;
            _root = null;
            DestroyOrImmediate(root.gameObject);

            IsInitialized = false;
        }

        public void CreatePopup(string key, string dataId, string contentTip, string confirmTip) =>
            CreatePopup(key, dataId, false, contentTip, confirmTip, string.Empty);

        public void CreatePopup(string key, string dataId, string contentTip, string confirmTip, string cancelTip) =>
            CreatePopup(key, dataId, true, contentTip, confirmTip, cancelTip);

        public void DestroyPopup(string key)
        {
            if (!IsInitialized)
                return;

            if (!_popupMapByKey.Remove(key, out var popup))
                return;

            popup.Release();
            DestroyOrImmediate(popup.GameObject);
        }

        public void DestroyAllPopups()
        {
            foreach (var key in _popupMapByKey.Keys.ToArray())
                DestroyPopup(key);
        }


        public async void ShowImmediately(string key) =>
            await ShowAsync(key, true);

        public async UniTask ShowAsync(string key) =>
            await ShowAsync(key, false);

        public async void HideImmediately(string key) =>
            await HideAsync(key, true);

        public async UniTask HideAsync(string key) =>
            await HideAsync(key, false);

        private void Select(string key, int index)
        {
            if (!IsInitialized)
                return;

            if (!TryGetIsVisibleAndIsNotConfirmPopup(key, out var popup))
                return;

            Select(popup, index);
        }

        public void MoveToForward(string key)
        {
            if (!IsInitialized)
                return;

            if (!TryGetIsVisibleAndIsNotConfirmPopup(key, out var popup))
                return;

            Select(key, popup.Index - 1);
        }

        public void MoveToBackward(string key)
        {
            if (!IsInitialized)
                return;

            if (!TryGetIsVisibleAndIsNotConfirmPopup(key, out var popup))
                return;

            Select(key, popup.Index + 1);
        }

        public UniTask ConfirmAsync(string key)
        {
            if (!IsInitialized)
                return UniTask.CompletedTask;

            if (!TryGetIsVisibleAndIsNotConfirmPopup(key, out var popup))
                return UniTask.CompletedTask;

            if (popup.Index == ConfrimIndex)
                return ConfirmPopupAsync(key);

            if (popup.Index == CancelIndex)
                return CancelPopupAsync(key);

            return UniTask.CompletedTask;
        }

        private UniTask ConfirmPopupAsync(string key)
        {
            if (!TryGetIsVisibleAndIsNotConfirmPopup(key, out var popup))
                return UniTask.CompletedTask;

            popup.SetIsConfirm(true);
            popup.Confirm();
            OnConfirm?.Invoke(key);

            return HideAsync(key);
        }

        private UniTask CancelPopupAsync(string key)
        {
            if (!TryGetIsVisibleAndIsNotConfirmPopup(key, out var popup))
                return UniTask.CompletedTask;

            popup.SetIsConfirm(true);
            popup.Cancel();
            OnCancel?.Invoke(key);

            return HideAsync(key);
        }

        private void CreatePopup(string key, string dataId, bool hasCancel, string contentTip, string confirmTip, string cancelTip)
        {
            if (!IsInitialized)
                return;

            if (!_popupModuleConfig.TryGetPopupPrefab(dataId, out var popupPrefab))
            {
                Debug.LogError($"[PopupModule::CreatePopup] Not find popupPrefab by DataId: {dataId}.");
                return;
            }

            if (_popupMapByKey.ContainsKey(key))
            {
                Debug.LogError($"[PopupModule::CreatePopup] Key: {key} already be created.");
                return;
            }

            var popupGameObject = Object.Instantiate(popupPrefab, _root);
            var popup = popupGameObject.GetComponent<IPopup>();

            popup.Initialize(key, dataId, hasCancel, contentTip, confirmTip, cancelTip, Select, ConfirmPopupAsync, CancelPopupAsync);

            _popupMapByKey.Add(key, popup);

            HideImmediately(key);
        }

        private async UniTask ShowAsync(string key, bool isImmediately)
        {
            if (!IsInitialized)
                return;

            if (!TryGetIsInvisiblePopup(key, out var popup))
                return;

            var contentText = m_GetTranslateText(popup.ContentTip);
            var confirmText = m_GetTranslateText(popup.ConfirmTip);
            var cancelText = m_GetTranslateText(popup.CancelTip);
            popup.SetText(contentText, confirmText, cancelText);

            popup.GameObject.SetActive(true);
            popup.SetIsConfirm(false);
            Select(popup, CancelIndex);
            
            popup.SetState(PopupStates.Showing);

            OnShowingStart?.Invoke(popup.Key);
            await popup.ShowAsync(isImmediately);
            OnShowingComplete?.Invoke(popup.Key);

            popup.SetState(PopupStates.Visible);

            string m_GetTranslateText(string m_tip) =>
                OnTranslate != null ? OnTranslate.Invoke(m_tip) : m_tip;
        }

        private async UniTask HideAsync(string key, bool isImmediately)
        {
            if (!IsInitialized)
                return;

            if (!TryGetIsVisiblePopup(key, out var popup))
                return;

            popup.SetState(PopupStates.Hiding);

            OnHidingStart?.Invoke(popup.Key);
            await popup.HideAsync(isImmediately);
            OnHidingComplete?.Invoke(popup.Key);

            popup.SetState(PopupStates.Invisible);

            popup.GameObject.SetActive(false);
        }

        private void Select(IPopup popup, int index)
        {
            var maxIndex = popup.HasCancel ? CancelIndex : ConfrimIndex;
            int selectedIndex;

            if (index < 0)
                selectedIndex = 0;

            else if (index > maxIndex)
                selectedIndex = maxIndex;

            else
                selectedIndex = index;

            popup.Select(selectedIndex);
        }

        private bool TryGetIsVisibleAndIsNotConfirmPopup(string key, out IPopup popup)
        {
            if (!TryGetIsVisiblePopup(key, out popup) || popup.IsConfirm)
                return false;

            return true;
        }

        private bool TryGetIsVisiblePopup(string key, out IPopup popup)
        {
            if (!_popupMapByKey.TryGetValue(key, out popup) || popup.State != PopupStates.Visible)
                return false;

            return true;
        }

        private bool TryGetIsInvisiblePopup(string key, out IPopup popup)
        {
            if (!_popupMapByKey.TryGetValue(key, out popup) || popup.State != PopupStates.Invisible)
                return false;

            return true;
        }

        private void DestroyOrImmediate(Object obj)
        {
            if (Application.isPlaying)
                Object.Destroy(obj);
            else
                Object.DestroyImmediate(obj);
        }
    }
}