using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaTransitionModule.Implement
{
    [Serializable]
    public class AnimSettings
    {
        [SerializeField]
        private string _animStateName = "Execute";

        [SerializeField]
        private Animator _animator;

        public void PlayAtStart(Action onPlay) =>
            _animator.PlayAtStart(_animStateName, 0, onPlay);

        public UniTask PlayAtStartAsync(Action onPlay) =>
            _animator.PlayAtStartAsync(_animStateName, 0, onPlay);
    }
}