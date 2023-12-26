using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaPopupModule
{
    public interface IPopup : IPopupReadModel
    {
        GameObject GameObject { get; }

        void Initialize(string key, string dataId, bool isAutoHideWhenConfirm, bool hasCancel, string contentTip, string confirmTip, string cancelTip, Action<string, int> onSelect, Func<string, UniTask> onConfirmPopupAsync, Func<string, UniTask> onCancelPopupAsync);
        void Release();

        void SetText(string contentText, string confirmText, string cancelText);

        void SetState(PopupStates state);
        UniTask ShowAsync(bool isImmediately);
        UniTask HideAsync(bool isImmediately);

        void SetHasConfirm(bool hasConfirm);

        void Select(int index);

        void Confirm();
        void Cancel();
    }
}