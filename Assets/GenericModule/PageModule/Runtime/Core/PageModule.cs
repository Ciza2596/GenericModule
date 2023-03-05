using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PageModule
{
    public class PageModule
    {
        //private variable
        private readonly PageContainer _pageContainer;

        //constructor
        public PageModule(IPageModuleConfig pageModuleConfig)
        {
            var pageGameObjectRootName = pageModuleConfig.PageGameObjectRootName;
            var pageGameObjectRoot = new GameObject(pageGameObjectRootName);

            var isDontDestroyOnLoad = pageModuleConfig.IsDontDestroyOnLoad;
            if (isDontDestroyOnLoad)
                Object.DontDestroyOnLoad(pageGameObjectRoot);

            var pageGameObjectRootTransform = pageGameObjectRoot.transform;
            var pagePrefabMap = pageModuleConfig.GetPagePrefabMap();

            _pageContainer = new PageContainer(pageGameObjectRootTransform, pagePrefabMap);

            var pageModuleComponent = pageGameObjectRoot.AddComponent<PageModuleComponent>();
            pageModuleComponent.SetUpdateCallback(_pageContainer.Update);
            pageModuleComponent.SetFixedUpdateCallback(_pageContainer.FixedUpdate);
        }


        //public method
        public void Release() => _pageContainer.Release();
        
        public bool CheckIsVisible<T>() where T : Component =>
            _pageContainer.CheckIsVisible<T>();

        public bool CheckIsShowing<T>() where T : Component =>
            _pageContainer.CheckIsShowing<T>();

        public bool CheckIsHiding<T>() where T : Component =>
            _pageContainer.CheckIsHiding<T>();


        public bool TryGetPage<T>(out T page) where T : Component =>
            _pageContainer.TryGetPage<T>(out page);


        public void Create<T>() where T : Component =>
            _pageContainer.Create<T>();

        public void CreateAll() => _pageContainer.CreateAll();


        public void Destroy<T>() where T : Component => _pageContainer.Destroy<T>();

        public void DestroyAll() => _pageContainer.DestroyAll();


        public async UniTask Show<T>(params object[] parameters) where T : Component =>
            await _pageContainer.Show<T>(parameters);

        public async UniTask ShowImmediately<T>(params object[] parameters) where T : Component =>
            await _pageContainer.ShowImmediately<T>(parameters);

        public async UniTask Show(Type[] pageTypes, object[][] parametersList) =>
            await _pageContainer.Show(pageTypes, parametersList);

        public async UniTask ShowImmediately(Type[] pageTypes, object[][] parametersList) =>
            await _pageContainer.ShowImmediately(pageTypes, parametersList);


        public async UniTask Hide<T>() where T : Component => await _pageContainer.Hide<T>();

        public void HideImmediately<T>() where T : Component => _pageContainer.HideImmediately<T>();


        public async UniTask Hide(Type[] pageTypes) => await _pageContainer.Hide(pageTypes);

        public void HideImmediately(Type[] pageTypes) => _pageContainer.HideImmediately(pageTypes);


        public async UniTask HideAll() => await _pageContainer.HideAll();

        public void HideAllImmediately() => _pageContainer.HideAllImmediately();
    }
}