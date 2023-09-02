using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaPageModule.Implement
{
    [CreateAssetMenu(fileName = "PageModuleConfig", menuName = "Ciza/PageModuleModule/PageModuleConfig")]
    public class PageModuleConfig : ScriptableObject, IPageModuleConfig
    {
        [SerializeField] private string _pageGameObjectRootName = "[PageRoot]";
        [SerializeField] private bool _isDontDestroyOnLoad = false;
        [Space] [SerializeField] private Page[] _pagePrefabs;


        public string PageGameObjectRootName => _pageGameObjectRootName;

        public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

        public Dictionary<Type, MonoBehaviour> GetPagePrefabMap()
        {
            var pagePrefabMap = new Dictionary<Type, MonoBehaviour>();

            foreach (var pagePrefab in _pagePrefabs)
            {
                Assert.IsNotNull(pagePrefab,
                    "[PageModuleConig::GetPagePrefabMap] Please check _pagePrefabs. Lose a pagePrefab.");
                var pageType = pagePrefab.GetType();
                pagePrefabMap.Add(pageType, pagePrefab);
            }

            return pagePrefabMap;
        }
    }
}