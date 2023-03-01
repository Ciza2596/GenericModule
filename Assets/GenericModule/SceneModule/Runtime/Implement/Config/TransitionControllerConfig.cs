using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace SceneModule.Implement
{
    [CreateAssetMenu(fileName = "TransitionControllerConfig", menuName = "SceneModule/TransitionControllerConfig")]
    public class TransitionControllerConfig : ScriptableObject, ITransitionControllerConfig
    {
        [SerializeField] private GameObject _viewParentPrefab;

        [SerializeField] private ViewPrefabMap[] _transitionInViewPrefabMaps;
        [SerializeField] private ViewPrefabMap[] _loadingViewPrefabMaps;
        [SerializeField] private ViewPrefabMap[] _transitionOutViewPrefabMaps;


        public GameObject GetViewParentPrefab() => _viewParentPrefab;


        public GameObject GetTransitionInViewPrefab(string viewName)
        {
            var viewPrefab = GetViewPrefab(_transitionInViewPrefabMaps, viewName);
            return viewPrefab;
        }

        public GameObject GetLoadingViewPrefab(string viewName)
        {
            var viewPrefab = GetViewPrefab(_loadingViewPrefabMaps, viewName);
            return viewPrefab;
        }

        public GameObject GetTransitionOutPrefab(string viewName)
        {
            var viewPrefab = GetViewPrefab(_transitionOutViewPrefabMaps, viewName);
            return viewPrefab;
        }


        private GameObject GetViewPrefab(ViewPrefabMap[] viewMaps, string viewName)
        {
            var viewMap = viewMaps.First(viewMap => viewMap.ViewName == viewName);
            Assert.IsNotNull(viewMap,
                $"[TransitionControllerConfig::GetViewPrefab] Not find prefab with viewName: {viewName}");

            var viewPrefab = viewMap.ViewPrefab;
            return viewPrefab;
        }
    }
}