using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaPopupModule
{
    public interface IPopup
    {
        string Key { get; }
        string DataId { get; }

        bool IsAutoHide { get; }
        bool HasCancel { get; }

        string ContentTip { get; }
        string ConfirmTip { get; }
        string CancelTip { get; }

        PopupStates State { get; }

        int Index { get; }
        bool IsConfirm { get; }

        GameObject GameObject { get; }

        void Initialize(string key, string dataId, bool isAutoHide, bool hasCancel, string contentTip, string confirmTip, string cancelTip, Action<string, int> onSelect, Func<string, UniTask> onConfirmPopupAsync, Func<string, UniTask> onCancelPopupAsync);
        void Release();

        void SetText(string contentText, string confirmText, string cancelText);

        void SetState(PopupStates state);
        UniTask ShowAsync(bool isImmediately);
        UniTask HideAsync(bool isImmediately);

        void SetIsConfirm(bool isConfirm);

        void Select(int index);

        void Confirm();
        void Cancel();
    }
}