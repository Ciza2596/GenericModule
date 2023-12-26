using System;
using System.Linq;
using CizaPopupModule.Implement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CizaPopupModule.Example
{
    public class PopupModuleExample : MonoBehaviour
    {
        [SerializeField]
        private PopupInfo[] _popupInfos;

        [Space]
        [SerializeField]
        private PopupModuleConfig _popupModuleConfig;

        private PopupModule _popupModule;

        private void Awake()
        {
            _popupModule = new PopupModule(_popupModuleConfig);
            _popupModule.Initialize();

            foreach (var popupInfo in _popupInfos.ToArray())
            {
                if (popupInfo.HasCancel)
                    _popupModule.CreatePopup(popupInfo.Key, popupInfo.DataId, true, popupInfo.ContentTip, popupInfo.ConfirmTip, popupInfo.CancelTip);
                else
                    _popupModule.CreatePopup(popupInfo.Key, popupInfo.DataId, true, popupInfo.ContentTip, popupInfo.ConfirmTip);
            }
        }


        [Button]
        private async void Show(string key) =>
            await _popupModule.ShowAsync(key);

        [Button]
        private async void Hide(string key) =>
            await _popupModule.HideAsync(key);


        [Serializable]
        private class PopupInfo
        {
            [SerializeField]
            private bool _hasCancel;

            [Space]
            [SerializeField]
            private string _key = "Default";

            [SerializeField]
            private string _dataId = "Default";

            [Space]
            [SerializeField]
            private string _contentTip = "Default_Content";

            [SerializeField]
            private string _confirmTip = "Default_Confirm";

            [SerializeField]
            private string _cancelTip = "Default_Cancel";

            public bool HasCancel => _hasCancel;

            public string Key => _key;
            public string DataId => _dataId;

            public string ContentTip => _contentTip;
            public string ConfirmTip => _confirmTip;
            public string CancelTip => _cancelTip;
        }
    }
}