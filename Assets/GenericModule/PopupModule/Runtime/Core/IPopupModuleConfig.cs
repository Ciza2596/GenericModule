using UnityEngine;

namespace CizaPopupModule
{
    public interface IPopupModuleConfig
    {
        string RootName { get; }
        bool IsDontDestroyOnLoad { get; }

        GameObject CanvasPrefab { get; }

        bool TryGetPopupPrefab(string dataId, out GameObject popupPrefab);
    }
}