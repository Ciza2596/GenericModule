
using System;
using UnityEngine;

namespace SceneModule.Implement
{
    [Serializable]
    public class ViewPrefabMap
    {
        [SerializeField]
        private string _viewName;

        [SerializeField] private GameObject _viewPrefab;

        public string ViewName => _viewName;
        public GameObject ViewPrefab => _viewPrefab;
    }
}
