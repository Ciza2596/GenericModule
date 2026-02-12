using System;
using CizaUniTask;
using UnityEngine;

namespace CizaTransitionModule.Implement
{
    public class LoadingAnimPage : LoadingPage
    {
        [Space]
        [SerializeField]
        private float _defaultLoadingTime = 0.5f;

        [Space]
        [SerializeField]
        private AnimSettings _animSettings;

        public override void PlayShowingAnimationImmediately() =>
            _animSettings.PlayAtStart(false, RefreshUI);


        public override UniTask DefaultLoadingAsync() =>
            UniTask.Delay(TimeSpan.FromSeconds(_defaultLoadingTime));
    }
}