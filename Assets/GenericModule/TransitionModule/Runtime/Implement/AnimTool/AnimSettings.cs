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

        public void PlayAtStart(bool isStop, Action onPlay) =>
            _animator.PlayAtStart(_animStateName, 0, isStop, onPlay);

        public UniTask PlayAtStartAsync(bool isStop, Action onPlay) =>
            _animator.PlayAtStartAsync(_animStateName, 0, isStop, onPlay);
    }
}