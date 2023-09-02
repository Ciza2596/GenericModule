using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaPageModule
{
	public class PageModule
	{
		//private variable
		private readonly PageContainer     _pageContainer;
		private readonly IPageModuleConfig _pageModuleConfig;

		//constructor
		public PageModule(IPageModuleConfig pageModuleConfig)
		{
			_pageContainer    = new PageContainer();
			_pageModuleConfig = pageModuleConfig;
		}

		//public variable
		public bool IsInitialized => _pageContainer.IsInitialized;

		//public method
		public void Initialize(Transform pageGameObjectRootParentTransform = null)
		{
			Release();

			var pageGameObjectRootName      = _pageModuleConfig.PageGameObjectRootName;
			var pageGameObjectRoot          = new GameObject(pageGameObjectRootName);
			var pageGameObjectRootTransform = pageGameObjectRoot.transform;

			var isDontDestroyOnLoad = _pageModuleConfig.IsDontDestroyOnLoad;
			if (isDontDestroyOnLoad)
				Object.DontDestroyOnLoad(pageGameObjectRoot);
			else if (pageGameObjectRootParentTransform != null)
				pageGameObjectRootTransform.SetParent(pageGameObjectRootParentTransform);

			var pagePrefabMap = _pageModuleConfig.GetPagePrefabMap();

			_pageContainer.Initialize(pageGameObjectRootTransform, pagePrefabMap);

			var pageModuleComponent = pageGameObjectRoot.AddComponent<PageModuleComponent>();
			pageModuleComponent.SetUpdateCallback(_pageContainer.Tick);
			pageModuleComponent.SetFixedUpdateCallback(_pageContainer.FixedTick);
		}

		public void Release() => _pageContainer.Release();

		public bool CheckIsVisible<T>() =>
			_pageContainer.CheckIsVisible<T>();

		public bool CheckIsShowing<T>() =>
			_pageContainer.CheckIsShowing<T>();

		public bool CheckIsHiding<T>() =>
			_pageContainer.CheckIsHiding<T>();

		public bool TryGetPage<T>(out T page) where T : class =>
			_pageContainer.TryGetPage<T>(out page);

		public void Create<TRegisteredPage, TPage>(params object[] parameters) where TPage : MonoBehaviour =>
			_pageContainer.Create<TRegisteredPage, TPage>(parameters);

		public void Create<T>(params object[] parameters) where T : MonoBehaviour =>
			_pageContainer.Create<T, T>(parameters);

		public void Destroy<T>() where T : MonoBehaviour => _pageContainer.Destroy<T>();

		public void DestroyAll() => _pageContainer.DestroyAll();

		public async UniTask Show<T>(Action onComplete = null, params object[] parameters) =>
			await _pageContainer.Show<T>(onComplete, parameters);

		public async UniTask ShowImmediately<T>(Action onComplete = null, params object[] parameters) =>
			await _pageContainer.ShowImmediately<T>(onComplete, parameters);

		public async UniTask Show(Type[] pageTypes, object[][] parametersList = null, Action onComplete = null) =>
			await _pageContainer.Show(pageTypes, parametersList, onComplete);

		public async UniTask ShowImmediately(Type[] pageTypes, object[][] parametersList = null, Action onComplete = null) =>
			await _pageContainer.ShowImmediately(pageTypes, parametersList, onComplete);

		public async UniTask Hide<T>(Action onComplete = null) =>
			await _pageContainer.Hide<T>(onComplete);

		public void HideImmediately<T>(Action onComplete = null) =>
			_pageContainer.HideImmediately<T>(onComplete);

		public async UniTask Hide(Type[] pageTypes, Action onComplete = null) =>
			await _pageContainer.Hide(pageTypes, onComplete);

		public void HideImmediately(Type[] pageTypes, Action onComplete = null) =>
			_pageContainer.HideImmediately(pageTypes, onComplete);

		public async UniTask HideAll(Action onComplete = null) => await _pageContainer.HideAll(onComplete);

		public void HideAllImmediately(Action onComplete = null) => _pageContainer.HideAllImmediately(onComplete);
	}
}
