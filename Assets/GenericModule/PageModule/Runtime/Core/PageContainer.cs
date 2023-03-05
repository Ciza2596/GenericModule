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

        private Action<float> _updateHandle;
        private Action<float> _fixedUpdateHandle;


        //constructor
        public PageContainer(Transform pageGameObjectRootTransform, Dictionary<Type, Component> pagePrefabMap)
        {
            _pageGameObjectRootTransform = pageGameObjectRootTransform;
            _pagePrefabMap = pagePrefabMap;
        }


        //Unity callback
        public void Update(float deltaTime) =>
            _updateHandle?.Invoke(deltaTime);

        public void FixedUpdate(float fixedDeltaTime) =>
            _fixedUpdateHandle?.Invoke(fixedDeltaTime);


        //public method
        public bool CheckIsVisible<T>() where T : Component
        {
            var pageType = typeof(T);
            if (!_pageDataMap.ContainsKey(pageType))
            {
                Debug.Log(
                    $"[PageContainer::CheckIsVisible] Not find pageType: {pageType} in pageMap.Please check it is created.");
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
                    $"[PageContainer::CheckIsShowing] Not find pageType: {pageType} in pageMap.Please check it is created.");
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
                    $"[PageContainer::CheckIsHiding] Not find pageType: {pageType} in pageMap.Please check it is created.");
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
                Debug.Log(
                    $"[PageContainer::Destroy] Not find pageType: {pageType} in pageMap.Please check it is created.");
                return false;
            }

            var pageData = _pageDataMap[pageType];
            page = pageData.Page as T;

            return true;
        }


        public void Create<T>(params object[] parameters) where T : Component =>
            Create(typeof(T), parameters);

        public void CreateAll()
        {
            var pageTypes = _pagePrefabMap.Keys.ToArray();
            foreach (var pageType in pageTypes)
                Create(pageType);
        }


        public void Destroy<T>() where T : Component =>
            Destroy(typeof(T));

        public void Destroy(Type pageType)
        {
            if (!_pageDataMap.ContainsKey(pageType))
            {
                Debug.Log(
                    $"[PageContainer::Destroy] Not find pageType: {pageType} in pageMap.Please check it is created.");
                return;
            }

            var pageData = _pageDataMap[pageType];
            _pageDataMap.Remove(pageType);

            pageData.Release();
        }

        public void DestroyAll()
        {
            var pageTypes = _pageDataMap.Keys.ToArray();
            foreach (var pageType in pageTypes)
                Destroy(pageType);
        }


        public async UniTask Show<T>(params object[] parameters) where T : Component =>
            await Show(typeof(T), false, parameters);

        public async UniTask ShowImmediately<T>(params object[] parameters) where T : Component =>
            await Show(typeof(T), true, parameters);


        public async UniTask Show(Type[] pageTypes, object[][] parametersList) =>
            await Show(pageTypes, false, parametersList);

        public async UniTask ShowImmediately(Type[] pageTypes, object[][] parametersList) =>
            await Show(pageTypes, true, parametersList);


        public async UniTask Hide<T>() where T : Component =>
            await Hide(typeof(T), false);

        public async void HideImmediately<T>() where T : Component =>
            await Hide(typeof(T), true);


        public async UniTask Hide(Type[] pageTypes) =>
            await Hide(pageTypes, false);

        public async void HideImmediately(Type[] pageTypes) =>
            await Hide(pageTypes, true);


        public async UniTask HideAll()
        {
            var pageDatas = _pageDataMap.Values.Where(pageData => pageData.State is PageState.Visible).ToArray();
            await Hide(pageDatas, false);
        }

        public async void HideAllImmediately()
        {
            var pageDatas = _pageDataMap.Values.Where(pageData => pageData.State is PageState.Visible).ToArray();
            await Hide(pageDatas, true);
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

        private async UniTask Show(Type pageType, bool isImmediately, params object[] parameters)
        {
            Assert.IsTrue(_pageDataMap.ContainsKey(pageType),
                $"[PageContainer::Show] PageType: {pageType} doesnt be created.");

            var pageData = _pageDataMap[pageType];

            var state = pageData.State;
            if (state != PageState.Invisible)
            {
                Debug.LogWarning(
                    $"[PageContainer::Hide] PageType: {pageType} is not Invisible. Current state is {state}.");
                return;
            }

            await pageData.BeforeShowing(parameters);

            pageData.Show();

            if (!isImmediately)
                await pageData.ShowingAction();

            pageData.CompleteShowing();

            AddUpdateAndFixedUpdateHandle(pageData);
        }

        private async UniTask Show(Type[] pageTypes, bool isImmediately, object[][] parametersList)
        {
            foreach (var pageType in pageTypes)
                Assert.IsTrue(_pageDataMap.ContainsKey(pageType),
                    $"[PageContainer::Show] PageType: {pageType} doesnt be created.");

            var beforeShowingTasks = new List<UniTask>();

            Action show = null;
            var showingActionTasks = new List<UniTask>();
            Action completeShowing = null;

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

                beforeShowingTasks.Add(pageData.BeforeShowing(parameters));

                show += pageData.Show;
                showingActionTasks.Add(pageData.ShowingAction());
                completeShowing += pageData.CompleteShowing;

                canShowPageDatas.Add(pageData);
            }

            await UniTask.WhenAll(beforeShowingTasks);

            show?.Invoke();

            if (!isImmediately)
                await UniTask.WhenAll(showingActionTasks);

            completeShowing?.Invoke();

            foreach (var canShowPageData in canShowPageDatas)
                AddUpdateAndFixedUpdateHandle(canShowPageData);
        }


        private async UniTask Hide(Type pageType, bool isImmediately)
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


            RemoveUpdateAndFixedUpdateHandle(pageData);

            pageData.Hide();

            if (!isImmediately)
                await pageData.HidingAction();

            pageData.CompleteHiding();
        }

        private async UniTask Hide(Type[] pageTypes, bool isImmediately)
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

            await Hide(canHidePageDatas.ToArray(), isImmediately);
        }

        private async UniTask Hide(PageData[] pageDatas, bool isImmediately)
        {
            Action hide = null;
            var hidingActionTasks = new List<UniTask>();
            Action completeHiding = null;

            foreach (var pageData in pageDatas)
            {
                var state = pageData.State;
                if (state != PageState.Visible)
                {
                    Debug.LogWarning(
                        $"[PageContainer::Hide] PageType: {pageDatas.GetType()} is not Visible. Current state is {state}.");
                    continue;
                }

                hide += pageData.Hide;
                hidingActionTasks.Add(pageData.HidingAction());
                completeHiding += pageData.CompleteHiding;
            }

            hide?.Invoke();

            if (!isImmediately)
                await UniTask.WhenAll(hidingActionTasks);

            completeHiding?.Invoke();

            foreach (var pageType in pageDatas)
                RemoveUpdateAndFixedUpdateHandle(pageType);
        }


        private void AddUpdateAndFixedUpdateHandle(PageData pageData)
        {
            if (pageData.TryGetUpdateable(out var updatable))
                _updateHandle += updatable.OnUpdate;

            if (pageData.TryGetFixedUpdateable(out var fixedUpdatable))
                _fixedUpdateHandle += fixedUpdatable.OnFixedUpdate;
        }

        private void RemoveUpdateAndFixedUpdateHandle(PageData pageData)
        {
            if (pageData.TryGetUpdateable(out var updatable))
                _updateHandle -= updatable.OnUpdate;

            if (pageData.TryGetFixedUpdateable(out var fixedUpdatable))
                _fixedUpdateHandle -= fixedUpdatable.OnFixedUpdate;
        }
    }
}