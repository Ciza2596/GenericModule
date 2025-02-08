namespace CizaPopupModule
{
    public interface IPopupReadModel
    {
        string Key { get; }
        string DataId { get; }

        bool IsAutoHideWhenConfirm { get; }
        bool HasCancel { get; }

        string ContentTip { get; }
        string ConfirmTip { get; }
        string CancelTip { get; }

        int DefaultButtonIndex { get; }

        PopupStates State { get; }

        int Index { get; }
        bool HasConfirm { get; }
    }
}