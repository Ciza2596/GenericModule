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
            pageModuleComponent.SetUpdateCallback(_pageContainer.Tick);
            pageModuleComponent.SetFixedUpdateCallback(_pageContainer.FixedTick);
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


        public void Create<T>(params object[] parameters) where T : Component =>
            _pageContainer.Create<T>(parameters);

        public void CreateAll(object[][] parametersList = null) =>
            _pageContainer.CreateAll(parametersList);


        public void Destroy<T>() where T : Component => _pageContainer.Destroy<T>();

        public void DestroyAll() => _pageContainer.DestroyAll();


        public async UniTask Show<T>(Action onComplete = null, params object[] parameters) where T : Component =>
            await _pageContainer.Show<T>(onComplete, parameters);

        public async UniTask ShowImmediately<T>(Action onComplete = null, params object[] parameters) where T : Component =>
            await _pageContainer.ShowImmediately<T>(onComplete, parameters);

        public async UniTask Show(Type[] pageTypes, object[][] parametersList, Action onComplete = null) =>
            await _pageContainer.Show(pageTypes, parametersList, onComplete);

        public async UniTask ShowImmediately(Type[] pageTypes, object[][] parametersList, Action onComplete = null) =>
            await _pageContainer.ShowImmediately(pageTypes, parametersList, onComplete);


        public async UniTask Hide<T>(Action onComplete = null) where T : Component => await _pageContainer.Hide<T>(onComplete);

        public void HideImmediately<T>(Action onComplete = null) where T : Component => _pageContainer.HideImmediately<T>(onComplete);


        public async UniTask Hide(Type[] pageTypes, Action onComplete = null) => await _pageContainer.Hide(pageTypes, onComplete);

        public void HideImmediately(Type[] pageTypes, Action onComplete = null) => _pageContainer.HideImmediately(pageTypes, onComplete);


        public async UniTask HideAll(Action onComplete = null) => await _pageContainer.HideAll(onComplete);

        public void HideAllImmediately(Action onComplete = null) => _pageContainer.HideAllImmediately(onComplete);
    }
}