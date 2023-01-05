
using System;
using SceneModule.Implement;
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
            
            gameObject.SetActive(true);
            
            await _animPlayer.Play();
            _onComplete?.Invoke();
            
            gameObject.SetActive(false);
        }
    }
}
