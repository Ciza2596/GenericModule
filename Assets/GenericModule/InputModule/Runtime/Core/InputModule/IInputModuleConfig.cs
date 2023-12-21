using UnityEngine;

namespace CizaInputModule
{
    public interface IInputModuleConfig
    {
        string RootName { get; }
        bool IsDontDestroyOnLoad { get; }

        GameObject EventSystemPrefab { get; }
        bool CanEnableEventSystem { get; }
        bool IsAutoEnableEventSystem { get; }

        GameObject PlayerInputManagerPrefab { get; }
        float JoinedWaitingTime { get; }

        string DefaultActionMapDataId { get; }
    }
}