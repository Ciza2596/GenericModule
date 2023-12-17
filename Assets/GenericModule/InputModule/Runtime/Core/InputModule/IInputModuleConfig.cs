using UnityEngine;

namespace CizaInputModule
{
    public interface IInputModuleConfig
    {
        string RootName { get; }

        bool IsDontDestroyOnLoad { get; }

        GameObject PlayerInputManagerPrefab { get; }
    }
}