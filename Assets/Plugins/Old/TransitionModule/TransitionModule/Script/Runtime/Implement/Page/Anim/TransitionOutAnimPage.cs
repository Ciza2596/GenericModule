using CizaUniTask;
using UnityEngine;

namespace CizaTransitionModule.Implement
{
    public class TransitionOutAnimPage : TransitionOutPage
    {
        [Space]
        [SerializeField]
        private AnimSettings _animSettings;

        public override UniTask PlayShowingAnimationAsync() =>
            _animSettings.PlayAtStartAsync(true, RefreshUI);
    }
}