using UnityEngine;

namespace SceneModule
{
    public interface ITransitionControllerConfig
    {
        public GameObject GetViewParentPrefab();

        public void SetIsViewName(bool isViewName);

        public GameObject GetTransitionInViewPrefab(string viewNameOrTag);

        public GameObject GetLoadingViewPrefab(string viewNameOrTag);

        public GameObject GetTransitionOutPrefab(string viewNameOrTag);
    }
}