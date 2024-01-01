using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace CizaPageModule
{
    public class PageModule
    {
        //private variable
        private readonly PageContainer _pageContainer;
        private readonly IPageModuleConfig _pageModuleConfig;

        public event Action<string> OnEnablePage;
        public event Action<string> OnDisablePage;

        public event Action<float> OnTick;

        //public variable
        public bool IsInitialized => _pageContainer.IsInitialized;

        //constructor
        [Preserve]
        public PageModule(IPageModuleConfig pageModuleConfig)
        {
            _pageContainer = new PageContainer();
            _pageModuleConfig = pageModuleConfig;

            _pageContainer.OnEnablePage += OnEnablePageImp;
            _pageContainer.OnDisablePage += OnDisablePageImp;

            _pageContainer.OnTick += OnTickImp;
        }

        //public method
        public void Initialize(Transform parent = null)
        {
            Release();

            var pageRootGameObject = new GameObject(_pageModuleConfig.PageRootName);
            var pageRoot = pageRootGameObject.transform;

            if (parent != null)
                pageRoot.SetParent(parent);

            else if (_pageModuleConfig.IsDontDestroyOnLoad)
                Object.DontDestroyOnLoad(pageRootGameObject);

            var pagePrefabs = _pageModuleConfig.GetPagePrefabs();

            _pageContainer.Initialize(pageRoot, pagePrefabs);

            var pageModuleComponent = pageRootGameObject.AddComponent<PageModuleMono>();
            pageModuleComponent.SetUpdateCallback(_pageContainer.Tick);
            pageModuleComponent.SetFixedUpdateCallback(_pageContainer.FixedTick);
        }

        public void Release() =>
            _pageContainer.Release();

        public bool CheckIsVisible(string key) =>
            _pageContainer.CheckIsVisible(key);

        public bool CheckIsShowing(string key) =>
            _pageContainer.CheckIsShowing(key);

        public bool CheckIsHiding(string key) =>
            _pageContainer.CheckIsHiding(key);

        public TPage[] GetAllPage<TPage>() where TPage : class =>
            _pageContainer.GetAllPage<TPage>();

        public bool TryGetPage<TPage>(string key, out TPage page) where TPage : class =>
            _pageContainer.TryGetPage(key, out page);

        public UniTask CreateAsync<TPage>(string key, params object[] parameters) where TPage : class =>
            _pageContainer.CreateAsync<TPage>(key, parameters);

        public void Destroy(string key) =>
            _pageContainer.Destroy(key);

        public void DestroyAll() =>
            _pageContainer.DestroyAll();

        public UniTask OnlyCallShowingPrepareAsync(string key, Action onComplete = null, params object[] parameters) =>
            _pageContainer.OnlyCallShowingPrepareAsync(key, onComplete, parameters);

        public UniTask ShowAsync(string key, Action onComplete = null, bool isIncludeShowingComplete = true, params object[] parameters) =>
            _pageContainer.ShowAsync(key, onComplete, isIncludeShowingComplete, parameters);

        public UniTask ShowImmediatelyAsync(string key, Action onComplete = null, bool isIncludeShowingComplete = true, params object[] parameters) =>
            _pageContainer.ShowImmediatelyAsync(key, onComplete, isIncludeShowingComplete, parameters);

        public void OnlyCallShowingComplete(string key, Action onComplete = null) =>
            _pageContainer.OnlyCallShowingComplete(key, onComplete);

        public UniTask ShowAsync(string[] keys, object[][] parametersList = null, Action onComplete = null, bool isIncludeHidingComplete = true) =>
            _pageContainer.ShowAsync(keys, parametersList, onComplete, isIncludeHidingComplete);

        public UniTask ShowImmediatelyAsync(string[] keys, object[][] parametersList = null, Action onComplete = null, bool isIncludeHidingComplete = true) =>
            _pageContainer.ShowImmediatelyAsync(keys, parametersList, onComplete, isIncludeHidingComplete);

        public void OnlyCallShowingComplete(string[] keys, Action onComplete = null) =>
            _pageContainer.OnlyCallShowingComplete(keys, onComplete);

        public void OnlyCallHidingStart(string key, Action onComplete = null) =>
            _pageContainer.OnlyCallHidingStart(key, onComplete);

        public UniTask HideAsync(string key, Action onComplete = null, bool isIncludeHidingComplete = true) =>
            _pageContainer.HideAsync(key, onComplete, isIncludeHidingComplete);

        public void HideImmediately(string key, Action onComplete = null, bool isIncludeHidingComplete = true) =>
            _pageContainer.HideImmediately(key, onComplete, isIncludeHidingComplete);

        public void OnlyCallHidingComplete(string key, Action onComplete = null) =>
            _pageContainer.OnlyCallHidingComplete(key, onComplete);

        public UniTask HideAsync(string[] keys, Action onComplete = null, bool isIncludeHidingComplete = true) =>
            _pageContainer.HideAsync(keys, onComplete, isIncludeHidingComplete);

        public void HideImmediately(string[] keys, Action onComplete = null, bool isIncludeHidingComplete = true) =>
            _pageContainer.HideImmediately(keys, onComplete, isIncludeHidingComplete);

        public void OnlyCallHidingComplete(string[] keys, Action onComplete = null) =>
            _pageContainer.OnlyCallHidingComplete(keys, onComplete);

        public UniTask HideAllAsync(Action onComplete = null, bool isIncludeHidingComplete = true) =>
            _pageContainer.HideAllAsync(onComplete, isIncludeHidingComplete);

        public void HideAllImmediately(Action onComplete = null, bool isIncludeHidingComplete = true) =>
            _pageContainer.HideAllImmediately(onComplete, isIncludeHidingComplete);

        public void OnlyCallAllHidingComplete(Action onComplete = null) =>
            _pageContainer.OnlyCallAllHidingComplete(onComplete);

        private void OnEnablePageImp(string pageKey) =>
            OnEnablePage?.Invoke(pageKey);

        private void OnDisablePageImp(string pageKey) =>
            OnDisablePage?.Invoke(pageKey);

        private void OnTickImp(float deltaTime) =>
            OnTick?.Invoke(deltaTime);
    }
}