using System.Collections.Generic;
using CizaPageModule.Implement;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaTransitionModule.Implement
{
    [CreateAssetMenu(fileName = "TransitionModuleConfig", menuName = "Ciza/TransitionModule/TransitionModuleConfig")]
    public class TransitionModuleConfig : ScriptableObject, ITransitionModuleConfig
    {
        [SerializeField]
        private string _rootName = "[TransitionModule]";

        [SerializeField]
        private bool _isDontDestroyOnLoad = true;

        [Space]
        [SerializeField]
        private Page[] _pagePrefabs;


        public string PageRootName => _rootName;
        public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

        public MonoBehaviour[] GetPagePrefabs()
        {
            var pagePrefabs = new List<MonoBehaviour>();

            foreach (var pagePrefab in _pagePrefabs)
            {
                Assert.IsNotNull(pagePrefab, "[TransitionModuleConfig::GetPagePrefabs] Please check pagePrefabs. Lose a pagePrefab.");
                pagePrefabs.Add(pagePrefab);
            }

            return pagePrefabs.ToArray();
        }
    }
}