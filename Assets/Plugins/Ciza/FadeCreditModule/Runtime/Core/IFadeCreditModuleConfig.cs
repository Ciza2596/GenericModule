using UnityEngine;

namespace CizaFadeCreditModule
{
    public interface IFadeCreditModuleConfig
    {
        string RootName { get; }

        bool IsDontDestroyOnLoad { get; }

        GameObject ControllerPrefab { get; }
    }
}