using System;
using System.Collections.Generic;
using UnityEngine;

namespace PageModule
{
    public interface IPageModuleConfig
    {
        public string PageGameObjectRootName { get; }
        public bool IsDontDestroyOnLoad { get; }

        public Dictionary<Type, Component> GetPagePrefabMap();
    }
}