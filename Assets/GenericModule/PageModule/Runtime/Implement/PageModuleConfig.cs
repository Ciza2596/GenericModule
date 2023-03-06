using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace PageModule.Implement
{
    [CreateAssetMenu(fileName = "PageModuleConfig", menuName = "PageModuleModule/PageModuleConfig")]
    public class PageModuleConfig : ScriptableObject, IPageModuleConfig
    {
        [SerializeField] private string _pageGameObjectRootName = "[PageRoot]";
        [SerializeField] private bool _isDontDestroyOnLoad = true;
        [Space] [SerializeField] private BasePage[] _pagePrefabs;


        public string PageGameObjectRootName => _pageGameObjectRootName;

        public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

        public Dictionary<Type, Component> GetPagePrefabMap()
        {
            var pagePrefabMap = new Dictionary<Type, Component>();

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