using UnityEngine;

namespace CizaInputModule
{
    public interface IInputModuleConfig
    {
        string RootName { get; }
        bool IsDontDestroyOnLoad { get; }

        GameObject EventSystemPrefab { get; }
        bool CanEnableEventSystem { get; }
        bool IsDefaultEnableEventSystem { get; }

        bool IsAutoHideEventSystem { get; }
        float AutoHideEventSystemTime { get; }

        GameObject PlayerInputManagerPrefab { get; }
        float JoinedWaitingTime { get; }

        string DefaultActionMapDataId { get; }
        string DisableActionMapDataId { get; }
    }
}