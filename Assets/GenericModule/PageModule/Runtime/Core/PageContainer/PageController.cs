using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaPageModule
{
	internal class PageController
	{
		//public variable
		public string Key { get; }

		public Component Page { get; }

		public PageState State { get; private set; }

		public bool IsAlreadyCallShowingPrepareAsync { get; private set; }

		public bool IsWorkingShowingPrepareAsync { get; private set; }
		public bool CanCallShowingComplete     { get; private set; }

		public bool IsAlreadyCallHidingStart { get; private set; }
		public bool CanCallHidingComplete    { get; private set; }

		//constructor
		public PageController(string key, Component page)
		{
			Key  = key;
			Page = page;
		}

		//public method
		public async UniTask InitializeAsync(params object[] parameters)
		{
			var pageGameObject = Page.gameObject;
			pageGameObject.SetActive(false);
			if (Page is IInitializable initializable)
				await initializable.InitializeAsync(parameters);
		}

		public void Release()
		{
			if (Page is IReleasable releasable)
				releasable.Release();

			Destroy();
		}

		public bool TryGetTickable(out ITickable tickable)
		{
			tickable = Page as ITickable;
			return tickable != null;
		}

		public bool TryGetFixedTickable(out IFixedTickable fixedTickable)
		{
			fixedTickable = Page as IFixedTickable;
			return fixedTickable != null;
		}

		public async UniTask OnShowingPrepareAsync(params object[] parameters)
		{
			State                          = PageState.Showing;
			IsAlreadyCallShowingPrepareAsync = true;

			IsWorkingShowingPrepareAsync = true;

			if (Page is IShowingPrepare showingPrepare)
				await showingPrepare.OnShowingPrepareAsync(parameters);

			IsWorkingShowingPrepareAsync = false;
		}
		
		public void EnablePage() =>
			Page.gameObject.SetActive(true);
		
		
		public void OnShowingStart()
		{
			if (Page is IShowingStart showingStart)
				showingStart.OnShowingStart();
		}

		public async UniTask PlayShowingAnimationAsync()
		{
			if (Page is IShowingAnimated showingAnimated)
				await showingAnimated.PlayShowingAnimationAsync();
		}

		public void PlayShowingAnimationImmediately()
		{
			if (Page is IShowingAnimatedImmediately showingAnimatedImmediately)
				showingAnimatedImmediately.PlayShowingAnimationImmediately();
		}

		public void EnableCanCallShowingComplete() =>
			CanCallShowingComplete = true;

		public void OnShowingComplete()
		{
			if (Page is IShowingComplete showingComplete)
				showingComplete.OnShowingComplete();

			State                          = PageState.Visible;
			CanCallShowingComplete         = false;
			IsAlreadyCallShowingPrepareAsync = false;
		}

		public void OnHidingStart()
		{
			State                    = PageState.Hiding;
			IsAlreadyCallHidingStart = true;

			if (Page is IHidingStart hidingStart)
				hidingStart.OnHidingStart();
		}

		public async UniTask PlayHidingAnimationAsync()
		{
			if (Page is IHidingAnimated hidingAnimated)
				await hidingAnimated.PlayHidingAnimationAsync();
		}

		public void PlayHidingAnimationImmediately()
		{
			if (Page is IHidingAnimatedImmediately hidingAnimatedImmediately)
				hidingAnimatedImmediately.PlayHidingAnimationImmediately();
		}

		public void EnableCanCallHidingComplete() =>
			CanCallHidingComplete = true;

		public void OnHidingComplete()
		{
			var pageGameObject = Page.gameObject;
			pageGameObject.SetActive(false);

			if (Page is IHidingComplete hidingComplete)
				hidingComplete.OnHidingComplete();

			State                    = PageState.Invisible;
			CanCallHidingComplete    = false;
			IsAlreadyCallHidingStart = false;
		}

		//private method
		private void Destroy()
		{
			var pageGameObject = Page.gameObject;

			if (Application.isPlaying)
				Object.Destroy(pageGameObject);
			else
				Object.DestroyImmediate(pageGameObject);
		}
	}
}
