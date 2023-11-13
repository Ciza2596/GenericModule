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

		private Transform       _pageGameObjectRootTransform;
		private MonoBehaviour[] _pagePrefabs;

		private Action<float> _tickHandle;
		private Action<float> _fixedTickHandle;

		//public variable
		public bool IsInitialized => _pageGameObjectRootTransform != null && _pagePrefabs != null;

		//Unity callback
		public void Tick(float deltaTime) =>
			_tickHandle?.Invoke(deltaTime);

		public void FixedTick(float fixedDeltaTime) =>
			_fixedTickHandle?.Invoke(fixedDeltaTime);

		//public method
		public void Initialize(Transform pageGameObjectRootTransform, MonoBehaviour[] pagePrefabs)
		{
			Release();

			_pageGameObjectRootTransform = pageGameObjectRootTransform;
			_pagePrefabs                 = pagePrefabs;
		}

		public void Release()
		{
			if (!IsInitialized)
				return;

			DestroyAll();

			_pagePrefabs = null;

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

		public TPage[] GetAllPage<TPage>() where TPage : class
		{
			var pages = new List<TPage>();

			foreach (var pageController in _pageControllerMapByKey.Values.ToArray())
				if (pageController.Page is TPage page)
					pages.Add(page);

			return pages.ToArray();
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

		public UniTask CreateAsync<TPage>(string key, params object[] parameters) where TPage : class =>
			CreateAsync(key, typeof(TPage), parameters);

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

		public async UniTask OnlyCallShowingStartAsync(string key, Action onComplete = null, params object[] parameters)
		{
			Assert.IsTrue(_pageControllerMapByKey.ContainsKey(key), $"[PageContainer::OnlyCallShowingStartAsync] Page: {key} doesnt be created.");

			var pageController = _pageControllerMapByKey[key];
			var state          = pageController.State;

			if (state != PageState.Invisible)
			{
				Debug.LogWarning($"[PageContainer::OnlyCallShowingStartAsync] Page: {key} should be Visible. Current state is {state}.");
				return;
			}

			if (pageController.IsAlreadyCallShowingStartAsync)
				return;

			await pageController.OnShowingStartAsync(parameters);
			onComplete?.Invoke();
		}

		public async UniTask ShowAsync(string key, Action onComplete = null, bool isIncludeShowingComplete = true, params object[] parameters) =>
			await ShowAsync(key, false, onComplete, isIncludeShowingComplete, "ShowAsync", parameters);

		public async UniTask ShowImmediatelyAsync(string key, Action onComplete = null, bool isIncludeShowingComplete = true, params object[] parameters) =>
			await ShowAsync(key, true, onComplete, isIncludeShowingComplete, "ShowAsync", parameters);

		public void OnlyCallShowingComplete(string key, Action onComplete = null)
		{
			Assert.IsTrue(_pageControllerMapByKey.ContainsKey(key), $"[PageContainer::OnlyCallShowingComplete] Page: {key} doesnt be created.");

			var pageController = _pageControllerMapByKey[key];
			var state          = pageController.State;
			if (state != PageState.Showing)
			{
				Debug.LogWarning($"[PageContainer::OnlyCallShowingComplete] Page: {key} should be showing. Current state is {state}.");
				return;
			}

			if (!pageController.CanCallShowingComplete)
				return;

			pageController.OnShowingComplete();
			AddTickAndFixedTickHandle(pageController);

			onComplete?.Invoke();
		}

		public async UniTask ShowAsync(string[] keys, object[][] parameters = null, Action onComplete = null, bool isIncludeHidingComplete = true) =>
			await ShowAsync(keys, false, onComplete, isIncludeHidingComplete, "ShowAsync", parameters);

		public async UniTask ShowImmediatelyAsync(string[] keys, object[][] parameters = null, Action onComplete = null, bool isIncludeHidingComplete = true) =>
			await ShowAsync(keys, true, onComplete, isIncludeHidingComplete, "ShowImmediatelyAsync", parameters);

		public void OnlyCallShowingComplete(string[] keys, Action onComplete = null)
		{
			foreach (var key in keys)
				OnlyCallShowingComplete(key);
			onComplete?.Invoke();
		}

		public void OnlyCallHidingStart(string key, Action onComplete = null)
		{
			Assert.IsTrue(_pageControllerMapByKey.ContainsKey(key), $"[PageContainer::OnlyCallHidingStart] Page: {key} doesnt be created.");

			var pageController = _pageControllerMapByKey[key];
			var state          = pageController.State;

			if (state != PageState.Visible)
			{
				Debug.LogWarning($"[PageContainer::OnlyCallHidingStart] Page: {key} should be Visible. Current state is {state}.");
				return;
			}

			if (pageController.IsAlreadyCallHidingStart)
				return;

			pageController.OnHidingStart();
			RemoveTickAndFixedTickHandle(pageController);

			onComplete?.Invoke();
		}

		public async UniTask HideAsync(string key, Action onComplete = null, bool isIncludeHidingComplete = true) =>
			await HideAsync(key, false, onComplete, isIncludeHidingComplete, "HideAsync");

		public async void HideImmediately(string key, Action onComplete = null, bool isIncludeHidingComplete = true) =>
			await HideAsync(key, true, onComplete, isIncludeHidingComplete, "HideAsync");

		public void OnlyCallHidingComplete(string key, Action onComplete = null)
		{
			Assert.IsTrue(_pageControllerMapByKey.ContainsKey(key), $"[PageContainer::OnlyCallHidingComplete] Page: {key} doesnt be created.");

			var pageController = _pageControllerMapByKey[key];
			var state          = pageController.State;
			if (state != PageState.Hiding)
			{
				Debug.LogWarning($"[PageContainer::OnlyCallHidingComplete] Page: {key} should be hiding. Current state is {state}.");
				return;
			}

			if (!pageController.CanCallHidingComplete)
				return;

			pageController.OnHidingComplete();
			onComplete?.Invoke();
		}

		public async UniTask HideAsync(string[] keys, Action onComplete = null, bool isIncludeHidingComplete = true) =>
			await HideAsync(keys, false, onComplete, isIncludeHidingComplete, "HideAsync", true);

		public async void HideImmediately(string[] keys, Action onComplete = null, bool isIncludeHidingComplete = true) =>
			await HideAsync(keys, true, onComplete, isIncludeHidingComplete, "HideImmediately", true);

		public void OnlyCallHidingComplete(string[] keys, Action onComplete = null)
		{
			foreach (var key in keys)
				OnlyCallHidingComplete(key);
			onComplete?.Invoke();
		}

		public async UniTask HideAllAsync(Action onComplete = null, bool isIncludeHidingComplete = true) =>
			await HideAsync(_pageControllerMapByKey.Keys.ToArray(), false, onComplete, isIncludeHidingComplete, "HideAllAsync", false);

		public async void HideAllImmediately(Action onComplete = null, bool isIncludeHidingComplete = true) =>
			await HideAsync(_pageControllerMapByKey.Keys.ToArray(), true, onComplete, isIncludeHidingComplete, "HideAllImmediately", false);

		public void OnlyCallAllHidingComplete(Action onComplete = null)
		{
			foreach (var pair in _pageControllerMapByKey.Where(pair => pair.Value.State == PageState.Hiding).ToArray())
				OnlyCallHidingComplete(pair.Key);
			onComplete?.Invoke();
		}

		//private method
		private async UniTask CreateAsync(string key, Type pageType, params object[] parameters)
		{
			if (_pageControllerMapByKey.ContainsKey(key))
			{
				Debug.LogWarning($"[PageContainer::Create] Page: {key} is created.");
				return;
			}

			if (!TryGetPrefab(pageType, out var pagePrefab))
			{
				Debug.LogWarning($"[PageContainer::Create] Not find pageType: {pageType} in pagePrefabs.");
				return;
			}

			var pageGameObjectPrefab = pagePrefab.gameObject;
			var pageGameObject       = Object.Instantiate(pageGameObjectPrefab, _pageGameObjectRootTransform);

			var page           = pageGameObject.GetComponent(pageType);
			var pageController = new PageController(key, page);

			await pageController.InitializeAsync(parameters);
			_pageControllerMapByKey.Add(key, pageController);
		}

		private async UniTask ShowAsync(string key, bool isImmediately, Action onComplete, bool isIncludeShowingComplete, string methodName, params object[] parameters)
		{
			Assert.IsTrue(_pageControllerMapByKey.ContainsKey(key), $"[PageContainer::{methodName}] Page: {key} doesnt be created.");

			var pageController = _pageControllerMapByKey[key];
			var state          = pageController.State;
			if (!(state == PageState.Invisible && !pageController.IsAlreadyCallShowingStartAsync) && !(state == PageState.Showing && pageController.IsAlreadyCallShowingStartAsync))
			{
				var expectedState = !pageController.IsAlreadyCallShowingStartAsync ? PageState.Invisible : PageState.Showing;
				Debug.LogWarning($"[PageContainer::{methodName}] Page: {key} should be {expectedState.ToString()}. Current state is {state}.");
				return;
			}

			if (!pageController.IsAlreadyCallShowingStartAsync)
				await pageController.OnShowingStartAsync(parameters);

			while (pageController.IsWorkingShowingStartAsync)
				await UniTask.Yield();

			if (!isImmediately)
				await pageController.PlayShowingAnimationAsync();

			if (isIncludeShowingComplete)
			{
				pageController.OnShowingComplete();
				AddTickAndFixedTickHandle(pageController);
			}
			else
				pageController.EnableCanCallShowingComplete();

			onComplete?.Invoke();
		}

		private async UniTask ShowAsync(string[] keys, bool isImmediately, Action onComplete, bool isIncludeShowingComplete, string methodName, object[][] parametersList)
		{
			var canShowPageControllers = new List<PageController>();
			foreach (var key in keys)
			{
				Assert.IsTrue(_pageControllerMapByKey.ContainsKey(key), $"[PageContainer::{methodName}] Page: {key} doesnt be created.");

				var pageController = _pageControllerMapByKey[key];
				var state          = pageController.State;

				if (!(state == PageState.Invisible && !pageController.IsAlreadyCallShowingStartAsync) && !(state == PageState.Showing && pageController.IsAlreadyCallShowingStartAsync))
				{
					var expectedState = !pageController.IsAlreadyCallShowingStartAsync ? PageState.Invisible : PageState.Showing;
					Debug.LogWarning($"[PageContainer::{methodName}] Page: {key} should be {expectedState.ToString()}. Current state is {state}.");
					continue;
				}

				canShowPageControllers.Add(pageController);
			}

			await m_Show(canShowPageControllers.ToArray(), isImmediately, onComplete, isIncludeShowingComplete, parametersList ?? new object[canShowPageControllers.Count][]);

			async UniTask m_Show(PageController[] m_pageControllers, bool m_isImmediately, Action m_onComplete, bool m_isIncludeShowingComplete, object[][] m_parametersList)
			{
				Func<UniTask> m_onShowingStart       = null;
				Func<UniTask> m_playShowingAnimation = null;
				Action        m_onShowingComplete    = null;

				for (var i = 0; i < m_pageControllers.Length; i++)
				{
					var m_pageController = m_pageControllers[i];
					var m_parameters     = m_parametersList[i];
					m_onShowingStart += async () =>
					{
						if (!m_pageController.IsAlreadyCallShowingStartAsync)
							await m_pageController.OnShowingStartAsync(m_parameters);

						while (m_pageController.IsWorkingShowingStartAsync)
							await UniTask.Yield();
					};
					m_playShowingAnimation += m_pageController.PlayShowingAnimationAsync;
					m_onShowingComplete += () =>
					{
						if (m_isIncludeShowingComplete)
							m_pageController.OnShowingComplete();

						else
							m_pageController.EnableCanCallShowingComplete();
					};
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

		private async UniTask HideAsync(string key, bool isImmediately, Action onComplete, bool isIncludeHidingComplete, string methodName)
		{
			Assert.IsTrue(_pageControllerMapByKey.ContainsKey(key), $"[PageContainer::{methodName}] Page: {key} doesnt be created.");

			var pageController = _pageControllerMapByKey[key];
			var state          = pageController.State;

			if (!(state == PageState.Visible && !pageController.IsAlreadyCallHidingStart) && !(state == PageState.Hiding && pageController.IsAlreadyCallHidingStart))
			{
				var expectedState = !pageController.IsAlreadyCallHidingStart ? PageState.Visible : PageState.Hiding;
				Debug.LogWarning($"[PageContainer::{methodName}] Page: {key} should be {expectedState.ToString()}. Current state is {state}.");
				return;
			}

			if (!pageController.IsAlreadyCallHidingStart)
			{
				pageController.OnHidingStart();
				RemoveTickAndFixedTickHandle(pageController);
			}

			if (!isImmediately)
				await pageController.PlayHidingAnimationAsync();

			if (isIncludeHidingComplete)
				pageController.OnHidingComplete();
			else
				pageController.EnableCanCallHidingComplete();

			onComplete?.Invoke();
		}

		private async UniTask HideAsync(string[] keys, bool isImmediately, Action onComplete, bool isIncludeHidingComplete, string methodName, bool isShowLog)
		{
			var canHidePageControllers = new List<PageController>();
			foreach (var key in keys)
			{
				Assert.IsTrue(_pageControllerMapByKey.ContainsKey(key), $"[PageContainer::{methodName}] Page: {key} doesnt be created.");

				var pageController = _pageControllerMapByKey[key];
				var state          = pageController.State;

				if (!(state == PageState.Visible && !pageController.IsAlreadyCallHidingStart) && !(state == PageState.Hiding && pageController.IsAlreadyCallHidingStart))
				{
					if (isShowLog)
					{
						var expectedState = !pageController.IsAlreadyCallHidingStart ? PageState.Visible : PageState.Hiding;
						Debug.LogWarning($"[PageContainer::{methodName}] Page: {key} should be {expectedState.ToString()}. Current state is {state}.");
					}

					continue;
				}

				canHidePageControllers.Add(pageController);
			}

			await m_Hide(canHidePageControllers.ToArray(), isImmediately, onComplete, isIncludeHidingComplete);

			async UniTask m_Hide(PageController[] m_canHidePageControllers, bool m_isImmediately, Action m_onComplete, bool m_isIncludeHidingComplete)
			{
				Action        m_onHidingStart       = null;
				Func<UniTask> m_playHidingAnimation = null;
				Action        m_onHidingComplete    = null;

				foreach (var m_canHidePageController in m_canHidePageControllers)
				{
					m_onHidingStart += () =>
					{
						if (!m_canHidePageController.IsAlreadyCallHidingStart)
						{
							m_canHidePageController.OnHidingStart();
							RemoveTickAndFixedTickHandle(m_canHidePageController);
						}
					};
					m_playHidingAnimation += m_canHidePageController.PlayHidingAnimationAsync;
					m_onHidingComplete += () =>
					{
						if (m_isIncludeHidingComplete)
							m_canHidePageController.OnHidingComplete();
						else
							m_canHidePageController.EnableCanCallHidingComplete();
					};
				}

				m_onHidingStart?.Invoke();

				if (!m_isImmediately)
					await WhenAll(m_playHidingAnimation);

				m_onHidingComplete?.Invoke();

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

		private bool TryGetPrefab(Type pageType, out MonoBehaviour mono)
		{
			foreach (var pagePrefab in _pagePrefabs)
			{
				if (pageType.IsInstanceOfType(pagePrefab))
				{
					mono = pagePrefab;
					return true;
				}
			}

			mono = null;
			return false;
		}
	}
}
