using System.Linq;
using UnityEngine;

namespace SceneModule.Example1
{
    [CreateAssetMenu(fileName = "TransitionControllerConfig", menuName = "SceneModule/TransitionControllerConfig")]
    public class TransitionControllerConfig : ScriptableObject, ITransitionControllerConfig
    {
        [SerializeField]
        private GameObject _viewParentPrefab;

        [SerializeField] private ViewMap[] _transitionInViewMaps;
        [SerializeField] private ViewMap[] _loadingViewMaps;
        [SerializeField] private ViewMap[] _transitionOutViewMaps;
        
        
        public GameObject GetViewParentPrefab() => _viewParentPrefab;

        
        public GameObject GetTransitionInViewPrefab(string viewName)
        {
            var viewPrefab = GetViewPrefab(_transitionInViewMaps, viewName);
            return viewPrefab;
        }

        public GameObject GetLoadingViewPrefab(string viewName)
        {
            var viewPrefab = GetViewPrefab(_loadingViewMaps, viewName);
            return viewPrefab;
        }

        public GameObject GetTransitionOutPrefab(string viewName)
        {
            var viewPrefab = GetViewPrefab(_transitionOutViewMaps, viewName);
            return viewPrefab;
        }


        private GameObject GetViewPrefab(ViewMap[] viewMaps, string viewName)
        {
            var viewMap = viewMaps.First(viewMap => viewMap.ViewName == viewName);
            var viewPrefab = viewMap.ViewPrefab;
            return viewPrefab;
        }

    }
}