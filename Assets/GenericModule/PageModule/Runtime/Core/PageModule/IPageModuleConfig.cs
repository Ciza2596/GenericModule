using System;
using System.Collections.Generic;
using UnityEngine;

namespace CizaPageModule
{
    public interface IPageModuleConfig
    {
        public string PageGameObjectRootName { get; }
        public bool IsDontDestroyOnLoad { get; }

        public Dictionary<Type, MonoBehaviour> GetPagePrefabMap();
    }
}