using UnityEngine;

namespace PageModule
{
    public class PageModule
    {
        private readonly Container _container;

        public PageModule(IPageModuleConfig pageModuleConfig)
        {
            var pageGameObjectRootName = pageModuleConfig.PageGameObjectRootName;
            var pageGameObjectRoot = new GameObject(pageGameObjectRootName);

            var isDontDestroyOnLoad = pageModuleConfig.IsDontDestroyOnLoad;
            if (isDontDestroyOnLoad)
                Object.DontDestroyOnLoad(pageGameObjectRoot);

            var pageGameObjectRootTransform = pageGameObjectRoot.transform;
            var pagePrefabMap = pageModuleConfig.GetPagePrefabMap();

            _container = new Container(pageGameObjectRootTransform, pagePrefabMap);

            var pageModuleComponent = pageGameObjectRoot.AddComponent<PageModuleComponent>();
            pageModuleComponent.SetUpdateCallback(_container.Update);
            pageModuleComponent.SetFixedUpdateCallback(_container.FixedUpdate);
        }
    }
}