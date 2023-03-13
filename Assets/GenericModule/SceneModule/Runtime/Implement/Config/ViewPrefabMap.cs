
using System;
using System.Linq;
using UnityEngine;

namespace CizaSceneModule.Implement
{
    [Serializable]
    public class ViewPrefabMap
    {
        [SerializeField]
        private string _viewName;

        [SerializeField] private string[] _viewTags;

        [SerializeField] private GameObject _viewPrefab;

        public string ViewName => _viewName;
        public string[] ViewTag => _viewTags.ToArray();
        public GameObject ViewPrefab => _viewPrefab;
    }
}
