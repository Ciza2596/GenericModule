using System;
using CizaUniTask;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaPageModule
{
	internal class PageController
	{
		private event Action<string> _onEnablePage;
		private event Action<string> _onDisablePage;

		//public variable
		public string Key { get; }

		public Component Page { get; }

		public PageStates State { get; private set; }

		public bool IsAlreadyCallShowingPrepareAsync { get; private set; }

		public bool IsWorkingShowingPrepareAsync { get; private set; }
		public bool CanCallShowingComplete       { get; private set; }

		public bool IsAlreadyCallHidingStart { get; private set; }
		public bool CanCallHidingComplete    { get; private set; }

		//constructor
		public PageController(string key, Component page, Action<string> onEnablePage, Action<string> onDisablePage)
		{
			Key  = key;
			Page = page;

			_onEnablePage  = onEnablePage;
			_onDisablePage = onDisablePage;
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

		public async UniTask ShowingPrepareAsync(params object[] parameters)
		{
			State                            = PageStates.Showing;
			IsAlreadyCallShowingPrepareAsync = true;

			IsWorkingShowingPrepareAsync = true;

			if (Page is IShowingPrepare showingPrepare)
				await showingPrepare.ShowingPrepareAsync(parameters);

			IsWorkingShowingPrepareAsync = false;
		}

		public void EnablePage()
		{
			Page.gameObject.SetActive(true);
			_onEnablePage?.Invoke(Key);
		}

		public void ShowingStart()
		{
			if (Page is IShowingStart showingStart)
				showingStart.ShowingStart();
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

		public void ShowingComplete()
		{
			if (Page is IShowingComplete showingComplete)
				showingComplete.ShowingComplete();

			State                            = PageStates.Visible;
			CanCallShowingComplete           = false;
			IsAlreadyCallShowingPrepareAsync = false;
		}

		public void HidingStart()
		{
			State                    = PageStates.Hiding;
			IsAlreadyCallHidingStart = true;

			if (Page is IHidingStart hidingStart)
				hidingStart.HidingStart();
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

		public void HidingComplete()
		{
			Page.gameObject.SetActive(false);
			_onDisablePage?.Invoke(Key);

			if (Page is IHidingComplete hidingComplete)
				hidingComplete.HidingComplete();

			State                    = PageStates.Invisible;
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
