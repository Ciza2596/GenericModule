

using UnityEngine;

namespace SceneModule
{
    public interface ITransitionControllerConfig
    {
        public GameObject GetViewParentPrefab();

        public GameObject GetTransitionInViewPrefab(string viewName);

        public GameObject GetLoadingViewPrefab(string viewName);

        public GameObject GetTransitionOutPrefab(string viewName);

    }
}
