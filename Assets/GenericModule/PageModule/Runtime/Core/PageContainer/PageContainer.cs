using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace PageModule
{
    public class PageContainer
    {
        //private variable
        private readonly Transform _pageGameObjectRootTransform;

        private readonly Dictionary<Type, Component> _pagePrefabMap;
        private readonly Dictionary<Type, PageData> _pageDataMap = new Dictionary<Type, PageData>();

        private Action<float> _tickHandle;
        private Action<float> _fixedTickHandle;


        //constructor
        public PageContainer(Transform pageGameObjectRootTransform, Dictionary<Type, Component> pagePrefabMap)
        {
            _pageGameObjectRootTransform = pageGameObjectRootTransform;
            _pagePrefabMap = pagePrefabMap;
        }


        //Unity callback
        public void Tick(float deltaTime) =>
            _tickHandle?.Invoke(deltaTime);

        public void FixedTick(float fixedDeltaTime) =>
            _fixedTickHandle?.Invoke(fixedDeltaTime);


        //public method
        public void Release()
        {
            DestroyAll();
            var pageGameObjectRoot = _pageGameObjectRootTransform.gameObject;
            DestroyOrImmediate(pageGameObjectRoot);
        }

        public bool CheckIsVisible<T>() where T : Component
        {
            var pageType = typeof(T);
            if (!_pageDataMap.ContainsKey(pageType))
            {
                Debug.Log(
                    $"[PageContainer::CheckIsVisible] Not find pageType: {pageType} in pageMap. Please check it is created.");
                return false;
            }

            var pageData = _pageDataMap[pageType];
            return pageData.State is PageState.Visible;
        }

        public bool CheckIsShowing<T>() where T : Component
        {
            var pageType = typeof(T);
            if (!_pageDataMap.ContainsKey(pageType))
            {
                Debug.Log(
                    $"[PageContainer::CheckIsShowing] Not find pageType: {pageType} in pageMap. Please check it is created.");
                return false;
            }

            var pageData = _pageDataMap[pageType];
            return pageData.State is PageState.Showing;
        }

        public bool CheckIsHiding<T>() where T : Component
        {
            var pageType = typeof(T);
            if (!_pageDataMap.ContainsKey(pageType))
            {
                Debug.Log(
                    $"[PageContainer::CheckIsHiding] Not find pageType: {pageType} in pageMap. Please check it is created.");
                return false;
            }

            var pageData = _pageDataMap[pageType];
            return pageData.State is PageState.Hiding;
        }


        public bool TryGetPage<T>(out T page) where T : Component
        {
            page = null;

            var pageType = typeof(T);
            if (!_pageDataMap.ContainsKey(pageType))
            {
                Debug.LogWarning(
                    $"[PageContainer::Destroy] Not find pageType: {pageType} in pageMap. Please check it is created.");
                return false;
            }

            var pageData = _pageDataMap[pageType];
            page = pageData.Page as T;

            return true;
        }


        public void Create<T>(params object[] parameters) where T : Component =>
            Create(typeof(T), parameters);

        public void CreateAll(object[][] parametersList = null)
        {
            var pageTypes = _pagePrefabMap.Keys.ToArray();
            var pageTypesLength = pageTypes.Length;
            if (parametersList is null)
                parametersList = new object[pageTypesLength][];

            for (var i = 0; i < pageTypesLength; i++)
            {
                var pageType = pageTypes[i];
                var parameters = parametersList[i];
                Create(pageType, parameters);
            }
        }


        public void Destroy<T>() where T : Component =>
            Destroy(typeof(T));

        public void DestroyAll()
        {
            var pageTypes = _pageDataMap.Keys.ToArray();
            foreach (var pageType in pageTypes)
                Destroy(pageType);
        }


        public async UniTask Show<T>(Action onComplete = null, params object[] parameters) where T : Component =>
            await Show(typeof(T), false, onComplete, parameters);

        public async UniTask ShowImmediately<T>(Action onComplete = null, params object[] parameters) where T : Component =>
            await Show(typeof(T), true, onComplete, parameters);


        public async UniTask Show(Type[] pageTypes, object[][] parametersList, Action onComplete = null) =>
            await Show(pageTypes, false, parametersList, onComplete);

        public async UniTask ShowImmediately(Type[] pageTypes, object[][] parametersList, Action onComplete = null) =>
            await Show(pageTypes, true, parametersList, onComplete);


        public async UniTask Hide<T>(Action onComplete = null) where T : Component =>
            await Hide(typeof(T), false, onComplete);

        public async void HideImmediately<T>(Action onComplete = null) where T : Component =>
            await Hide(typeof(T), true, onComplete);

        public async UniTask Hide(Type[] pageTypes, Action onComplete = null) =>
            await Hide(pageTypes, false, onComplete);

        public async void HideImmediately(Type[] pageTypes, Action onComplete = null) =>
            await Hide(pageTypes, true, onComplete);


        public async UniTask HideAll(Action onComplete = null)
        {
            var pageDatas = _pageDataMap.Values.Where(pageData => pageData.State is PageState.Visible).ToArray();
            await Hide(pageDatas, false, onComplete);
        }

        public async void HideAllImmediately(Action onComplete = null)
        {
            var pageDatas = _pageDataMap.Values.Where(pageData => pageData.State is PageState.Visible).ToArray();
            await Hide(pageDatas, true, onComplete);
        }


        //private method
        private void Create(Type pageType, params object[] parameters)
        {
            Assert.IsTrue(_pagePrefabMap.ContainsKey(pageType),
                $"[PageContainer::Create] Not find pageType: {pageType} in pagePrefabComponentMap.");

            if (_pageDataMap.ContainsKey(pageType))
            {
                Debug.LogWarning($"[PageContainer::Create] PageType: {pageType} is created.");
                return;
            }

            var pagePrefab = _pagePrefabMap[pageType];
            var pageGameObjectPrefab = pagePrefab.gameObject;
            var pageGameObject = Object.Instantiate(pageGameObjectPrefab, _pageGameObjectRootTransform);

            var page = pageGameObject.GetComponent(pageType);
            var pageData = new PageData(page);

            pageData.Initialize(parameters);
            _pageDataMap.Add(pageType, pageData);
        }


        private void Destroy(Type pageType)
        {
            if (!_pageDataMap.ContainsKey(pageType))
            {
                Debug.Log(
                    $"[PageContainer::Destroy] Not find pageType: {pageType} in pageMap. Please check it is created.");
                return;
            }

            var pageData = _pageDataMap[pageType];
            _pageDataMap.Remove(pageType);

            if (pageData.State is PageState.Invisible)
                RemoveTickAndFixedTickHandle(pageData);

            pageData.Release();
        }

        private async UniTask Show(Type pageType, bool isImmediately, Action onComplete, params object[] parameters)
        {
            Assert.IsTrue(_pageDataMap.ContainsKey(pageType),
                $"[PageContainer::Show] PageType: {pageType} doesnt be created.");

            var pageData = _pageDataMap[pageType];

            var state = pageData.State;
            if (state != PageState.Invisible)
            {
                Debug.LogWarning(
                    $"[PageContainer::Show] PageType: {pageType} is not Invisible. Current state is {state}.");
                return;
            }

            await pageData.OnShowingStart(parameters);

            if (!isImmediately)
                await pageData.PlayShowingAnimation();

            pageData.OnShowingComplete();

            AddTickAndFixedTickHandle(pageData);
            
            onComplete?.Invoke();
        }

        private async UniTask Show(Type[] pageTypes, bool isImmediately, object[][] parametersList, Action onComplete)
        {
            Func<UniTask> onShowingStart = null;
            Func<UniTask> playShowingAnimation = null;
            Action onShowingComplete = null;

            var canShowPageDatas = new List<PageData>();


            for (int i = 0; i < pageTypes.Length; i++)
            {
                var pageType = pageTypes[i];
                Assert.IsTrue(_pageDataMap.ContainsKey(pageType),
                    $"[PageContainer::Show] PageType: {pageType} doesnt be created.");

                var parameters = parametersList[i];
                var pageData = _pageDataMap[pageType];
                var state = pageData.State;
                if (state != PageState.Invisible)
                {
                    Debug.LogWarning(
                        $"[Container::Show] PageType: {pageType} is not Invisible. Current state is {state}.");
                    continue;
                }

                onShowingStart += async () => await pageData.OnShowingStart(parameters);
                playShowingAnimation += pageData.PlayShowingAnimation;
                onShowingComplete += pageData.OnShowingComplete;

                canShowPageDatas.Add(pageData);
            }

            await WhenAll(onShowingStart);

            if (!isImmediately)
                await WhenAll(playShowingAnimation);


            onShowingComplete?.Invoke();

            foreach (var canShowPageData in canShowPageDatas)
                AddTickAndFixedTickHandle(canShowPageData);
            
            onComplete?.Invoke();
        }


        private async UniTask Hide(Type pageType, bool isImmediately, Action onComplete)
        {
            Assert.IsTrue(_pageDataMap.ContainsKey(pageType),
                $"[PageContainer::Hide] PageType: {pageType} doesnt be created.");

            var pageData = _pageDataMap[pageType];

            var state = pageData.State;
            if (state != PageState.Visible)
            {
                Debug.LogWarning(
                    $"[PageContainer::Hide] PageType: {pageType} is not Visible. Current state is {state}.");
                return;
            }


            RemoveTickAndFixedTickHandle(pageData);

            pageData.OnHidingStart();

            if (!isImmediately)
                await pageData.PlayHidingAnimation();

            pageData.OnHidingComplete();

            onComplete?.Invoke();
        }

        private async UniTask Hide(Type[] pageTypes, bool isImmediately, Action onComplete)
        {
            var canHidePageDatas = new List<PageData>();

            foreach (var pageType in pageTypes)
            {
                Assert.IsTrue(_pageDataMap.ContainsKey(pageType),
                    $"[PageContainer::Hide] PageType: {pageType} doesnt be created.");

                var pageData = _pageDataMap[pageType];

                var state = pageData.State;
                if (state != PageState.Visible)
                {
                    Debug.LogWarning(
                        $"[PageContainer::Hide] PageType: {pageType} is not Visible. Current state is {state}.");
                    continue;
                }

                canHidePageDatas.Add(pageData);
            }

            await Hide(canHidePageDatas.ToArray(), isImmediately, onComplete);
        }

        private async UniTask Hide(PageData[] pageDatas, bool isImmediately, Action onComplete)
        {
            Action onHidingStart = null;
            Func<UniTask> playHidingAnimation = null;
            Action onHidingComplete = null;

            foreach (var pageData in pageDatas)
            {
                var state = pageData.State;
                if (state != PageState.Visible)
                {
                    Debug.LogWarning(
                        $"[PageContainer::Hide] PageType: {pageDatas.GetType()} is not Visible. Current state is {state}.");
                    continue;
                }

                onHidingStart += pageData.OnHidingStart;
                playHidingAnimation += pageData.PlayHidingAnimation;
                onHidingComplete += pageData.OnHidingComplete;
            }

            onHidingStart?.Invoke();

            if (!isImmediately)
                await WhenAll(playHidingAnimation);

            onHidingComplete?.Invoke();

            foreach (var pageType in pageDatas)
                RemoveTickAndFixedTickHandle(pageType);

            onComplete?.Invoke();
        }


        private void AddTickAndFixedTickHandle(PageData pageData)
        {
            if (pageData.TryGetTickable(out var tickable))
                _tickHandle += tickable.Tick;

            if (pageData.TryGetFixedTickable(out var fixedTickable))
                _fixedTickHandle += fixedTickable.FixedTick;
        }

        private void RemoveTickAndFixedTickHandle(PageData pageData)
        {
            if (pageData.TryGetTickable(out var tickable))
                _tickHandle -= tickable.Tick;

            if (pageData.TryGetFixedTickable(out var fixedTickable))
                _fixedTickHandle -= fixedTickable.FixedTick;
        }

        private void DestroyOrImmediate(Object obj)
        {
            if (Application.isPlaying)
                Object.Destroy(obj);
            else
                Object.DestroyImmediate(obj);
        }

        private async UniTask WhenAll(Func<UniTask> funcs)
        {
            if (funcs is null)
                return;

            var uniTasks = new List<UniTask>();
            foreach (var invocation in funcs.GetInvocationList())
                uniTasks.Add(((Func<UniTask>)invocation).Invoke());

            await UniTask.WhenAll(uniTasks);
        }
    }
}