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

        void Initialize(string key, string dataId, bool hasCancel, string contentTip, string confirmTip, string cancelTip);
        void Release();

        void SetText(string contentText, string confirmText, string cancelText);

        UniTask ShowAsync(bool isImmediately);
        UniTask HideAsync(bool isImmediately);

        void Select(int index);
        
        void Confirm(int index);
        
        void Confirm();
        void Cancel();
    }
}