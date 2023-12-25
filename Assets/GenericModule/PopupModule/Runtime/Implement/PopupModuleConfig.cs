using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaPopupModule.Implement
{
    [CreateAssetMenu(fileName = "PopupModuleConfig", menuName = "PopupModule/PopupModuleConfig")]
    public class PopupModuleConfig : ScriptableObject, IPopupModuleConfig
    {
        [SerializeField]
        private string _rootName = "[PopupModule]";

        [SerializeField]
        private bool _isDontDestroyOnLoad = true;

        [Space]
        [SerializeField]
        private GameObject _canvasPrefab;

        [SerializeField]
        private PopupPrefabMap[] _popupPrefabMaps;

        public string RootName => _rootName;
        public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

        public GameObject CanvasPrefab
        {
            get
            {
                Assert.IsNotNull(_canvasPrefab, $"[PopupModuleConfig::CanvasPrefab] CanvasPrefab is null. Please check popupModuleConfig: {name}.");
                return _canvasPrefab;
            }
        }

        public bool TryGetPopupPrefab(string dataId, out GameObject popupPrefab)
        {
            if (_popupPrefabMaps == null || _popupPrefabMaps.Length == 0)
            {
                popupPrefab = null;
                return false;
            }

            var popupMap = _popupPrefabMaps.FirstOrDefault(popupMap => popupMap.DataId == dataId);
            if (popupMap != null && popupMap.Prefab != null)
            {
                popupPrefab = popupMap.Prefab;
                return true;
            }

            popupPrefab = null;
            return false;
        }


        [Serializable]
        private class PopupPrefabMap
        {
            [SerializeField]
            private string _dataId;

            [SerializeField]
            private GameObject _prefab;

            public string DataId => _dataId;

            public GameObject Prefab => _prefab;
        }
    }
}