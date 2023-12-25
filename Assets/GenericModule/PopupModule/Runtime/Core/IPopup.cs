using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaPopupModule
{
    public interface IPopup
    {
        string Key { get; }
        string DataId { get; }
        bool HasCancel { get; }

        string ContentTip { get; }
        string ConfirmTip { get; }
        string CancelTip { get; }

        PopupStates State { get; }

        int Index { get; }
        bool IsConfirm { get; }

        GameObject GameObject { get; }

        void Initialize(string key, string dataId, bool hasCancel, string contentTip, string confirmTip, string cancelTip, Func<string, UniTask> onConfirmPopupAsync, Func<string, UniTask> onCancelPopupAsync);
        void Release();

        void SetText(string contentText, string confirmText, string cancelText);

        UniTask ShowAsync(bool isImmediately);
        UniTask HideAsync(bool isImmediately);

        void SetIsConfirm(bool isConfirm);

        void SetIndex(int index);

        void Confirm();
        void Cancel();
    }
}