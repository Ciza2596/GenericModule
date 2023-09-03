using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace CizaPageModule
{
	public class PageContainer
	{
		//private variable
		private readonly Dictionary<Type, PageController> _pageDataMap = new Dictionary<Type, PageController>();

		private Transform                       _pageGameObjectRootTransform;
		private Dictionary<Type, MonoBehaviour> _pagePrefabMap;

		private Action<float> _tickHandle;
		private Action<float> _fixedTickHandle;

		//public variable
		public bool IsInitialized => _pageGameObjectRootTransform != null && _pagePrefabMap != null;

		//Unity callback
		public void Tick(float deltaTime) =>
			_tickHandle?.Invoke(deltaTime);

		public void FixedTick(float fixedDeltaTime) =>
			_fixedTickHandle?.Invoke(fixedDeltaTime);

		//public method
		public void Initialize(Transform pageGameObjectRootTransform, Dictionary<Type, MonoBehaviour> pagePrefabMap)
		{
			Release();

			_pageGameObjectRootTransform = pageGameObjectRootTransform;
			_pagePrefabMap               = pagePrefabMap;
		}

		public void Release()
		{
			if (!IsInitialized)
				return;

			DestroyAll();

			_pagePrefabMap = null;

			var pageGameObjectRoot = _pageGameObjectRootTransform.gameObject;
			_pageGameObjectRootTransform = null;
			DestroyOrImmediate(pageGameObjectRoot);
		}

		public bool CheckIsVisible<T>()
		{
			var pageType = typeof(T);
			if (!_pageDataMap.ContainsKey(pageType))
			{
				Debug.Log($"[PageContainer::CheckIsVisible] Not find pageType: {pageType} in pageMap. Please check it is created.");
				return false;
			}

			var pageData = _pageDataMap[pageType];
			return pageData.State is PageState.Visible;
		}

		public bool CheckIsShowing<T>()
		{
			var pageType = typeof(T);
			if (!_pageDataMap.ContainsKey(pageType))
			{
				Debug.Log($"[PageContainer::CheckIsShowing] Not find pageType: {pageType} in pageMap. Please check it is created.");
				return false;
			}

			var pageData = _pageDataMap[pageType];
			return pageData.State is PageState.Showing;
		}

		public bool CheckIsHiding<T>()
		{
			var pageType = typeof(T);
			if (!_pageDataMap.ContainsKey(pageType))
			{
				Debug.Log($"[PageContainer::CheckIsHiding] Not find pageType: {pageType} in pageMap. Please check it is created.");
				return false;
			}

			var pageData = _pageDataMap[pageType];
			return pageData.State is PageState.Hiding;
		}

		public bool TryGetPage<T>(out T page) where T : class
		{
			page = default;

			var pageType = typeof(T);
			if (!_pageDataMap.ContainsKey(pageType))
			{
				Debug.LogWarning($"[PageContainer::Destroy] Not find pageType: {pageType} in pageMap. Please check it is created.");
				return false;
			}

			var pageData = _pageDataMap[pageType];
			page = pageData.Page as T;

			return true;
		}

		public void Create<TRegisteredPage, TPage>(params object[] parameters) where TPage : MonoBehaviour =>
			Create(typeof(TRegisteredPage), typeof(TPage), parameters);

		public void CreateAll(params object[][] parametersList)
		{
			var pageTypes       = _pagePrefabMap.Keys.ToArray();
			var pageTypesLength = pageTypes.Length;
			if (parametersList.Length != pageTypesLength)
				parametersList = new object[pageTypesLength][];

			for (var i = 0; i < pageTypesLength; i++)
			{
				var pageType   = pageTypes[i];
				var parameters = parametersList[i];
				Create(pageType, pageType, parameters);
			}
		}

		public void Destroy<T>() =>
			Destroy(typeof(T));

		public void DestroyAll()
		{
			var pageTypes = _pageDataMap.Keys.ToArray();
			foreach (var pageType in pageTypes)
				Destroy(pageType);
		}

		public async UniTask Show<T>(Action onComplete = null, params object[] parameters) =>
			await Show(typeof(T), false, onComplete, parameters);

		public async UniTask ShowImmediately<T>(Action onComplete = null, params object[] parameters) =>
			await Show(typeof(T), true, onComplete, parameters);

		public async UniTask Show(Type[] pageTypes, object[][] parametersList = null, Action onComplete = null) =>
			await Show(pageTypes, false, parametersList, onComplete);

		public async UniTask ShowImmediately(Type[] pageTypes, object[][] parametersList = null, Action onComplete = null) =>
			await Show(pageTypes, true, parametersList, onComplete);

		public async UniTask Hide<T>(Action onComplete = null) =>
			await Hide(typeof(T), false, onComplete);

		public async void HideImmediately<T>(Action onComplete = null) =>
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
		private void Create(Type registeredPageType, Type pageType, params object[] parameters)
		{
			Assert.IsTrue(_pagePrefabMap.ContainsKey(pageType), $"[PageContainer::Create] Not find pageType: {pageType} in pagePrefabComponentMap.");

			if (_pageDataMap.ContainsKey(registeredPageType))
			{
				Debug.LogWarning($"[PageContainer::Create] PageType: {registeredPageType} is created.");
				return;
			}

			if (pageType.Name != registeredPageType.Name && !pageType.GetInterfaces().Contains(registeredPageType))
			{
				Debug.LogWarning($"[PageContainer::Create] {pageType.Name} doesn't inherit {registeredPageType.Name}.");
				return;
			}

			var pagePrefab           = _pagePrefabMap[pageType];
			var pageGameObjectPrefab = pagePrefab.gameObject;
			var pageGameObject       = Object.Instantiate(pageGameObjectPrefab, _pageGameObjectRootTransform);

			var page     = pageGameObject.GetComponent(pageType);
			var pageData = new PageController(page);

			pageData.Initialize(parameters);
			_pageDataMap.Add(registeredPageType, pageData);
		}

		private void Destroy(Type pageType)
		{
			if (!_pageDataMap.ContainsKey(pageType))
			{
				Debug.Log($"[PageContainer::Destroy] Not find pageType: {pageType} in pageMap. Please check it is created.");
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
			Assert.IsTrue(_pageDataMap.ContainsKey(pageType), $"[PageContainer::Show] PageType: {pageType} doesnt be created.");

			var pageData = _pageDataMap[pageType];
			var state    = pageData.State;
			if (state != PageState.Invisible)
			{
				Debug.LogWarning($"[PageContainer::Show] PageType: {pageType} is not Invisible. Current state is {state}.");
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
			Func<UniTask> onShowingStart       = null;
			Func<UniTask> playShowingAnimation = null;
			Action        onShowingComplete    = null;

			var canShowPageDatas = new List<PageController>();

			var pageTypesLength = pageTypes.Length;
			if (parametersList is null || parametersList.Length != pageTypesLength)
				parametersList = new object[pageTypesLength][];

			for (int i = 0; i < pageTypes.Length; i++)
			{
				var pageType = pageTypes[i];
				Assert.IsTrue(_pageDataMap.ContainsKey(pageType), $"[PageContainer::Show] PageType: {pageType} doesnt be created.");

				var parameters = parametersList[i];
				var pageData   = _pageDataMap[pageType];
				var state      = pageData.State;
				if (state != PageState.Invisible)
				{
					Debug.LogWarning($"[Container::Show] PageType: {pageType} is not Invisible. Current state is {state}.");
					continue;
				}

				onShowingStart       += async () => await pageData.OnShowingStart(parameters);
				playShowingAnimation += pageData.PlayShowingAnimation;
				onShowingComplete    += pageData.OnShowingComplete;

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
			Assert.IsTrue(_pageDataMap.ContainsKey(pageType), $"[PageContainer::Hide] PageType: {pageType} doesnt be created.");

			var pageData = _pageDataMap[pageType];
			var state    = pageData.State;
			if (state != PageState.Visible)
			{
				Debug.LogWarning($"[PageContainer::Hide] PageType: {pageType} is not Visible. Current state is {state}.");
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
			var canHidePageDatas = new List<PageController>();

			foreach (var pageType in pageTypes)
			{
				Assert.IsTrue(_pageDataMap.ContainsKey(pageType), $"[PageContainer::Hide] PageType: {pageType} doesnt be created.");

				var pageData = _pageDataMap[pageType];
				var state    = pageData.State;
				if (state != PageState.Visible)
				{
					Debug.LogWarning($"[PageContainer::Hide] PageType: {pageType} is not Visible. Current state is {state}.");
					continue;
				}

				canHidePageDatas.Add(pageData);
			}

			await Hide(canHidePageDatas.ToArray(), isImmediately, onComplete);
		}

		private async UniTask Hide(PageController[] pageDatas, bool isImmediately, Action onComplete)
		{
			Action        onHidingStart       = null;
			Func<UniTask> playHidingAnimation = null;
			Action        onHidingComplete    = null;

			foreach (var pageData in pageDatas)
			{
				var state = pageData.State;
				if (state != PageState.Visible)
				{
					Debug.LogWarning($"[PageContainer::Hide] PageType: {pageDatas.GetType()} is not Visible. Current state is {state}.");
					continue;
				}

				onHidingStart       += pageData.OnHidingStart;
				playHidingAnimation += pageData.PlayHidingAnimation;
				onHidingComplete    += pageData.OnHidingComplete;
			}

			onHidingStart?.Invoke();

			if (!isImmediately)
				await WhenAll(playHidingAnimation);

			onHidingComplete?.Invoke();

			foreach (var pageType in pageDatas)
				RemoveTickAndFixedTickHandle(pageType);

			onComplete?.Invoke();
		}

		private void AddTickAndFixedTickHandle(PageController pageController)
		{
			if (pageController.TryGetTickable(out var tickable))
				_tickHandle += tickable.Tick;

			if (pageController.TryGetFixedTickable(out var fixedTickable))
				_fixedTickHandle += fixedTickable.FixedTick;
		}

		private void RemoveTickAndFixedTickHandle(PageController pageController)
		{
			if (pageController.TryGetTickable(out var tickable))
				_tickHandle -= tickable.Tick;

			if (pageController.TryGetFixedTickable(out var fixedTickable))
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
