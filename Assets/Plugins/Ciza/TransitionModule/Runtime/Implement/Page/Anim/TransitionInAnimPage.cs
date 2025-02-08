using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaTransitionModule.Implement
{
    public class TransitionInAnimPage : TransitionInPage
    {
        [Space]
        [SerializeField]
        private AnimSettings _animSettings;

        public override UniTask PlayShowingAnimationAsync() =>
            _animSettings.PlayAtStartAsync(true, RefreshUI);
    }
}