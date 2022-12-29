
using System;
using UnityEngine;

namespace SceneModule.Example1
{
    [Serializable]
    public class ViewMap
    {
        [SerializeField]
        private string _viewName;

        [SerializeField] private GameObject _viewPrefab;

        public string ViewName => _viewName;
        public GameObject ViewPrefab => _viewPrefab;
    }
}
