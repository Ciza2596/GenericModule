using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace PageModule
{
    public class Container
    {
        //private variable
        private readonly Transform _pageGameObjectRootTransform;

        private readonly Dictionary<Type, Component> _pagePrefabMap;
        private readonly Dictionary<Type, PageData> _pageDataMap = new Dictionary<Type, PageData>();

        private Action<float> _updateHandle;
        private Action<float> _fixedUpdateHandle;


        //constructor
        public Container(Transform pageGameObjectRootTransform, Dictionary<Type, Component> pagePrefabMap)
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
            if (!_pageDataMap.ContainsKey(typeof(T)))
            {
                Debug.Log(
                    $"[PageModule::CheckIsVisible] Not find pageType: {pageType} in pageMap.Please check it is created.");
                return false;
            }

            return true;
        }

        public bool CheckIsShowing<T>() where T : Component
        {
            var pageType = typeof(T);
            if (!_pageDataMap.ContainsKey(typeof(T)))
            {
                Debug.Log(
                    $"[PageModule::CheckIsShowing] Not find pageType: {pageType} in pageMap.Please check it is created.");
                return false;
            }

            return true;
        }

        public bool CheckIsHiding<T>() where T : Component
        {
            var pageType = typeof(T);
            if (!_pageDataMap.ContainsKey(typeof(T)))
            {
                Debug.Log(
                    $"[PageModule::CheckIsHiding] Not find pageType: {pageType} in pageMap.Please check it is created.");
                return false;
            }

            return true;
        }

        public bool TryGetPage<T>(out T page) where T : Component
        {
            page = null;

            var pageType = typeof(T);
            if (!_pageDataMap.ContainsKey(typeof(T)))
            {
                Debug.Log(
                    $"[PageModule::Destroy] Not find pageType: {pageType} in pageMap.Please check it is created.");
                return false;
            }

            var pageData = _pageDataMap[pageType];
            page = pageData.Page as T;

            return true;
        }


        public void Create<T>() where T : Component =>
            Create(typeof(T));

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
                    $"[PageModule::Destroy] Not find pageType: {pageType} in pageMap.Please check it is created.");
                return;
            }

            var pageData = _pageDataMap[pageType];
            _pageDataMap.Remove(pageType);

            var pageGameObject = pageData.PageGameObject;
            Object.DestroyImmediate(pageGameObject);
        }

        public void DestroyAll()
        {
            var pageTypes = _pageDataMap.Keys.ToArray();
            foreach (var pageType in pageTypes)
                Destroy(pageType);
        }


        public async UniTask Show<T>(params object[] parameters) where T : Component
        {
            await UniTask.CompletedTask;
        }

        public void ShowImmediately<T>(params object[] parameters) where T : Component
        {
        }


        public async UniTask Hide<T>() where T : Component
        {
            await UniTask.CompletedTask;
        }

        public void HideImmediately<T>() where T : Component
        {
        }


        public async UniTask HideAll()
        {
            await UniTask.CompletedTask;
        }

        public void HideAllImmediately()
        {
        }


        //private method
        private void Create(Type pageType)
        {
            Assert.IsTrue(_pagePrefabMap.ContainsKey(pageType),
                $"[PageModule::Create] Not find pageType: {pageType} in pagePrefabComponentMap.");

            var pagePrefab = _pagePrefabMap[pageType];
            var pageGameObjectPrefab = pagePrefab.gameObject;
            var pageGameObject = Object.Instantiate(pageGameObjectPrefab, _pageGameObjectRootTransform);

            var page = pageGameObject.GetComponent(pageType);
            var pageData = new PageData(page);
            _pageDataMap.Add(pageType, pageData);
        }
    }
}