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

			var pagePrefabs = _pageModuleConfig.GetPagePrefabs();

			_pageContainer.Initialize(pageGameObjectRootTransform, pagePrefabs);

			var pageModuleComponent = pageGameObjectRoot.AddComponent<PageModuleMono>();
			pageModuleComponent.SetUpdateCallback(_pageContainer.Tick);
			pageModuleComponent.SetFixedUpdateCallback(_pageContainer.FixedTick);
		}

		public void Release() => _pageContainer.Release();

		public bool CheckIsVisible(string key) =>
			_pageContainer.CheckIsVisible(key);

		public bool CheckIsShowing(string key) =>
			_pageContainer.CheckIsShowing(key);

		public bool CheckIsHiding(string key) =>
			_pageContainer.CheckIsHiding(key);

		public bool TryGetPage<T>(string key, out T page) where T : class =>
			_pageContainer.TryGetPage<T>(key, out page);

		public UniTask Create<TPage>(string key, params object[] parameters) where TPage : MonoBehaviour =>
			_pageContainer.Create<TPage>(key, parameters);

		public void Destroy(string key) =>
			_pageContainer.Destroy(key);

		public void DestroyAll() => _pageContainer.DestroyAll();

		public async UniTask Show(string key, Action onComplete = null) =>
			await _pageContainer.Show(key, onComplete);

		public async UniTask ShowImmediately(string key, Action onComplete = null) =>
			await _pageContainer.ShowImmediately(key, onComplete);

		public async UniTask Show(string[] keys, Action onComplete = null) =>
			await _pageContainer.Show(keys, onComplete);

		public async UniTask ShowImmediately(string[] keys, Action onComplete = null) =>
			await _pageContainer.ShowImmediately(keys, onComplete);

		public async UniTask Hide(string key, Action onComplete = null) =>
			await _pageContainer.Hide(key, onComplete);

		public void HideImmediately(string key, Action onComplete = null) =>
			_pageContainer.HideImmediately(key, onComplete);

		public async UniTask Hide(string[] keys, Action onComplete = null) =>
			await _pageContainer.Hide(keys, onComplete);

		public void HideImmediately(string[] keys, Action onComplete = null) =>
			_pageContainer.HideImmediately(keys, onComplete);

		public async UniTask HideAll(Action onComplete = null) => await _pageContainer.HideAll(onComplete);

		public void HideAllImmediately(Action onComplete = null) => _pageContainer.HideAllImmediately(onComplete);
	}
}
