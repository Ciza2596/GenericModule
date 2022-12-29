
using System;
using UnityEngine;

namespace SceneModule.Example1
{
    public class AnimTransitionView : BaseTransitionView
    {
        [SerializeField]
        private BaseAnimPlayer _animPlayer;
        
        public override async void Play(Action onComplete)
        {
            base.Play(onComplete);
            
            await _animPlayer.Play();
            
            _onComplete?.Invoke();
        }
    }
}
