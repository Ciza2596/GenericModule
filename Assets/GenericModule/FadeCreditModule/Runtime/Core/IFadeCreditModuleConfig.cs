using UnityEngine;

namespace CizaFadeCreditModule
{
    public interface IFadeCreditModuleConfig
    {
        bool IsDontDestroyOnLoad { get; }

        GameObject ControllerPrefab { get; }
    }
}