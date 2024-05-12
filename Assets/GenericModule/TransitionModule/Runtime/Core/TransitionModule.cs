using System;
using System.Collections.Generic;
using CizaPageModule;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaTransitionModule
{
    public class TransitionModule
    {
        //private variable
        private readonly PageModule _pageModule;

        private IPresenter[] _currentPresenters;
        private IPresenter[] _nextPresenters;

        //public variable
        public const string TransitionInPageDataId = "TransitionIn";
        public const string LoadingPageDataId = "Loading";
        public const string TransitionOutPageDataId = "TransitionOut";

        public bool CanTransit { get; private set; }

        public bool IsInitialized => _pageModule.IsInitialized;

        public IPresenter[] CurrentPresenters => _currentPresenters ?? Array.Empty<IPresenter>();
        public IPresenter[] NextPresenters => _nextPresenters ?? Array.Empty<IPresenter>();


        [Preserve]
        public TransitionModule(ITransitionModuleConfig transitionModuleConfig) =>
            _pageModule = new PageModule(transitionModuleConfig);

        public void Initialize(Transform parent)
        {
            if (IsInitialized)
                return;

            _pageModule.Initialize(parent);
            CanTransit = true;
            SetCurrentPresentersToBeNull();
            SetNextPresentersToBeNull();
        }

        public void Release()
        {
            if (!IsInitialized)
                return;

            _pageModule.Release();
            SetCurrentPresentersToBeNull();
            SetNextPresentersToBeNull();
        }

        public UniTask TransitAsync(IPresenter nextPresenter) =>
            TransitAsync(new[] { nextPresenter });

        public UniTask TransitAsync(IPresenter[] nextPresenters) =>
            TransitAsync(TransitionInPageDataId, LoadingPageDataId, TransitionOutPageDataId, nextPresenters);


        private async UniTask TransitAsync(string transitionInPageDataId, string loadingPageDataId, string transitionOutPageDataId, IPresenter[] nextPresenters)
        {
            if (!IsInitialized || !CanTransit)
                return;

            CanTransit = false;
            SetNextPresenters(nextPresenters);

            await CreateAllPagesAsync(transitionInPageDataId, loadingPageDataId, transitionOutPageDataId);

            await TransitionInAsync(transitionInPageDataId);
            await LoadingAsync(transitionInPageDataId, loadingPageDataId);
            await TransitionOutAsync(loadingPageDataId, transitionOutPageDataId);
            _pageModule.DestroyAll();
            CanTransit = true;
        }

        private void SetCurrentPresentersToBeNull() =>
            SetCurrentPresenters(null);

        private void SetCurrentPresenters(IPresenter[] currentPresenters) =>
            _currentPresenters = currentPresenters;

        private void SetNextPresentersToBeNull() =>
            SetNextPresenters(null);

        private void SetNextPresenters(IPresenter[] nextPresenters) =>
            _nextPresenters = nextPresenters;


        private async UniTask CreateAllPagesAsync(string transitionInPageDataId, string loadingPageDataId, string transitionOutPageDataId)
        {
            await _pageModule.CreateAsync<ITransitionInPage>(transitionInPageDataId);
            await _pageModule.CreateAsync<ILoadingPage>(loadingPageDataId);
            await _pageModule.CreateAsync<ITransitionOutPage>(transitionOutPageDataId);
        }

        private UniTask TransitionInAsync(string transitionInPageDataId) =>
            _pageModule.ShowAsync(transitionInPageDataId);


        private async UniTask LoadingAsync(string transitionInPageDataId, string loadingPageDataId)
        {
            await _pageModule.ShowImmediatelyAsync(loadingPageDataId);
            _pageModule.HideImmediately(transitionInPageDataId);

            if (!_pageModule.TryGetPage<ILoadingPage>(loadingPageDataId, out var loadingPage))
                return;

            var uniTasks = new List<UniTask>();
            uniTasks.Add(NextPresenters.InitializeAsync());
            uniTasks.Add(loadingPage.DefaultLoadingAsync());
            await UniTask.WhenAll(uniTasks);

            CurrentPresenters.Release();
            NextPresenters.Complete();
            SetCurrentPresenters(NextPresenters);
            SetNextPresentersToBeNull();
        }


        private async UniTask TransitionOutAsync(string loadingPageDataId, string transitionOutPageDataId)
        {
            await _pageModule.OnlyCallShowingPrepareAsync(transitionOutPageDataId);
            _pageModule.HideImmediately(loadingPageDataId);
            await _pageModule.ShowAsync(transitionOutPageDataId);
        }
    }
}