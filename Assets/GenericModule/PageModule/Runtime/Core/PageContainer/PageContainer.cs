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
		private readonly Dictionary<string, PageController> _pageControllerMapByKey = new Dictionary<string, PageController>();

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

		public bool CheckIsVisible(string key)
		{
			if (!_pageControllerMapByKey.ContainsKey(key))
			{
				Debug.Log($"[PageContainer::CheckIsVisible] Not find page: {key} in pageControllerMapByKey. Please check it is created.");
				return false;
			}

			var pageController = _pageControllerMapByKey[key];
			return pageController.State is PageState.Visible;
		}

		public bool CheckIsShowing(string key)
		{
			if (!_pageControllerMapByKey.ContainsKey(key))
			{
				Debug.Log($"[PageContainer::CheckIsShowing] Not find page: {key} in pageControllerMapByKey. Please check it is created.");
				return false;
			}

			var pageController = _pageControllerMapByKey[key];
			return pageController.State is PageState.Showing;
		}

		public bool CheckIsHiding(string key)
		{
			if (!_pageControllerMapByKey.ContainsKey(key))
			{
				Debug.Log($"[PageContainer::CheckIsHiding] Not find page: {key} in pageControllerMapByKey. Please check it is created.");
				return false;
			}

			var pageController = _pageControllerMapByKey[key];
			return pageController.State is PageState.Hiding;
		}

		public bool TryGetPage<T>(string key, out T page) where T : class
		{
			if (!_pageControllerMapByKey.ContainsKey(key))
			{
				page = null;
				return false;
			}

			var pageController = _pageControllerMapByKey[key];
			page = pageController.Page as T;
			return true;
		}

		public UniTask Create<TPage>(string key, params object[] parameters) where TPage : MonoBehaviour =>
			Create(key, typeof(TPage), parameters);

		public void Destroy(string key)
		{
			if (!_pageControllerMapByKey.ContainsKey(key))
			{
				Debug.Log($"[PageContainer::Destroy] Not find page: {key} in pageControllerMapByKey. Please check it is created.");
				return;
			}

			var pageController = _pageControllerMapByKey[key];
			_pageControllerMapByKey.Remove(key);

			if (pageController.State is PageState.Invisible)
				RemoveTickAndFixedTickHandle(pageController);

			pageController.Release();
		}

		public void DestroyAll()
		{
			var keys = _pageControllerMapByKey.Keys.ToArray();
			foreach (var key in keys)
				Destroy(key);
		}

		public async UniTask Show(string key, Action onComplete = null) =>
			await Show(key, false, onComplete);

		public async UniTask ShowImmediately(string key, Action onComplete = null) =>
			await Show(key, true, onComplete);

		public async UniTask Show(string[] keys, Action onComplete = null) =>
			await Show(keys, false, onComplete);

		public async UniTask ShowImmediately(string[] keys, Action onComplete = null) =>
			await Show(keys, true, onComplete);

		public async UniTask Hide(string key, Action onComplete = null) =>
			await Hide(key, false, onComplete);

		public async void HideImmediately(string key, Action onComplete = null) =>
			await Hide(key, true, onComplete);

		public async UniTask Hide(string[] keys, Action onComplete = null) =>
			await Hide(keys, false, onComplete, true);

		public async void HideImmediately(string[] keys, Action onComplete = null) =>
			await Hide(keys, true, onComplete, true);

		public async UniTask HideAll(Action onComplete = null) =>
			await Hide(_pageControllerMapByKey.Keys.ToArray(), false, onComplete, false);

		public async void HideAllImmediately(Action onComplete = null) =>
			await Hide(_pageControllerMapByKey.Keys.ToArray(), true, onComplete, false);

		//private method
		private async UniTask Create(string key, Type pageType, params object[] parameters)
		{
			Assert.IsTrue(_pagePrefabMap.ContainsKey(pageType), $"[PageContainer::Create] Not find pageType: {pageType} in pagePrefabMap.");

			if (_pageControllerMapByKey.ContainsKey(key))
			{
				Debug.LogWarning($"[PageContainer::Create] Page: {key} is created.");
				return;
			}

			var pagePrefab           = _pagePrefabMap[pageType];
			var pageGameObjectPrefab = pagePrefab.gameObject;
			var pageGameObject       = Object.Instantiate(pageGameObjectPrefab, _pageGameObjectRootTransform);

			var page           = pageGameObject.GetComponent(pageType);
			var pageController = new PageController(key, page);

			await pageController.Initialize(parameters);
			_pageControllerMapByKey.Add(key, pageController);
		}

		private async UniTask Show(string key, bool isImmediately, Action onComplete)
		{
			Assert.IsTrue(_pageControllerMapByKey.ContainsKey(key), $"[PageContainer::Show] Page: {key} doesnt be created.");

			var pageController = _pageControllerMapByKey[key];
			var state          = pageController.State;
			if (state != PageState.Invisible)
			{
				Debug.LogWarning($"[PageContainer::Show] Page: {key} is not Invisible. Current state is {state}.");
				return;
			}

			await pageController.OnShowingStart();

			if (!isImmediately)
				await pageController.PlayShowingAnimation();

			pageController.OnShowingComplete();
			AddTickAndFixedTickHandle(pageController);

			onComplete?.Invoke();
		}

		private async UniTask Show(string[] keys, bool isImmediately, Action onComplete)
		{
			var canShowPageControllers = new List<PageController>();
			foreach (var key in keys)
			{
				Assert.IsTrue(_pageControllerMapByKey.ContainsKey(key), $"[PageContainer::Show] Page: {key} doesnt be created.");

				var pageController = _pageControllerMapByKey[key];
				var state          = pageController.State;
				if (state != PageState.Invisible)
				{
					Debug.LogWarning($"[Container::Show] Page: {key} is not invisible. Current state is {state}.");
					continue;
				}

				canShowPageControllers.Add(pageController);
			}

			await m_Show(canShowPageControllers.ToArray(), isImmediately, onComplete);

			async UniTask m_Show(PageController[] m_pageControllers, bool m_isImmediately, Action m_onComplete)
			{
				Func<UniTask> m_onShowingStart       = null;
				Func<UniTask> m_playShowingAnimation = null;
				Action        m_onShowingComplete    = null;

				foreach (var m_pageController in m_pageControllers)
				{
					m_onShowingStart       += async () => await m_pageController.OnShowingStart();
					m_playShowingAnimation += m_pageController.PlayShowingAnimation;
					m_onShowingComplete    += m_pageController.OnShowingComplete;
				}

				await WhenAll(m_onShowingStart);

				if (!m_isImmediately)
					await WhenAll(m_playShowingAnimation);

				m_onShowingComplete?.Invoke();

				foreach (var m_canShowPageController in canShowPageControllers)
					AddTickAndFixedTickHandle(m_canShowPageController);

				m_onComplete?.Invoke();
			}
		}

		private async UniTask Hide(string key, bool isImmediately, Action onComplete)
		{
			Assert.IsTrue(_pageControllerMapByKey.ContainsKey(key), $"[PageContainer::Hide] Page: {key} doesnt be created.");

			var pageController = _pageControllerMapByKey[key];
			var state          = pageController.State;
			if (state != PageState.Visible)
			{
				Debug.LogWarning($"[PageContainer::Hide] Page: {key} is not Visible. Current state is {state}.");
				return;
			}

			RemoveTickAndFixedTickHandle(pageController);
			pageController.OnHidingStart();

			if (!isImmediately)
				await pageController.PlayHidingAnimation();

			pageController.OnHidingComplete();
			onComplete?.Invoke();
		}

		private async UniTask Hide(string[] keys, bool isImmediately, Action onComplete, bool isShowLog)
		{
			var canHidePageControllers = new List<PageController>();
			foreach (var key in keys)
			{
				Assert.IsTrue(_pageControllerMapByKey.ContainsKey(key), $"[PageContainer::Hide] Page: {key} doesnt be created.");

				var pageController = _pageControllerMapByKey[key];
				var state          = pageController.State;
				if (state != PageState.Visible)
				{
					if (isShowLog)
						Debug.LogWarning($"[PageContainer::Hide] Page: {key} is not Visible. Current state is {state}.");

					continue;
				}

				canHidePageControllers.Add(pageController);
			}

			await m_Hide(canHidePageControllers.ToArray(), isImmediately, onComplete);

			async UniTask m_Hide(PageController[] m_canHidePageControllers, bool m_isImmediately, Action m_onComplete)
			{
				Action        m_onHidingStart       = null;
				Func<UniTask> m_playHidingAnimation = null;
				Action        m_onHidingComplete    = null;

				foreach (var m_canHidePageController in m_canHidePageControllers)
				{
					m_onHidingStart       += m_canHidePageController.OnHidingStart;
					m_playHidingAnimation += m_canHidePageController.PlayHidingAnimation;
					m_onHidingComplete    += m_canHidePageController.OnHidingComplete;
				}

				m_onHidingStart?.Invoke();

				if (!m_isImmediately)
					await WhenAll(m_playHidingAnimation);

				m_onHidingComplete?.Invoke();

				foreach (var m_canHidePageController in m_canHidePageControllers)
					RemoveTickAndFixedTickHandle(m_canHidePageController);

				m_onComplete?.Invoke();
			}
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
