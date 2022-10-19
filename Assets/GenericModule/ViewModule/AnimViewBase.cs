
using System;
using UnityEngine;

namespace ViewModule
{
    public abstract class AnimViewBase : ViewBase
    {
        //private variable
        [SerializeField] private Settings _animSettings;

        
        //viewBase callback
        protected override void OnUpdateStart(float deltaTime)
        {
            UpdateShowView();
            UpdateHideView();
        }


        //protected method
        protected override void OnShowStart(params object[] items)
        {
            _animSettings.Animator.Play(_animSettings.ShowAnimName);
        }

        protected override void OnHideStart()
        {
            _animSettings.Animator.Play(_animSettings.HideAnimName);
        }

        //private method
        private void UpdateShowView()
        {
            if(!IsShowing)
                return;

            var stateInfo = _animSettings.Animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(_animSettings.ShowAnimName) && stateInfo.normalizedTime >
                _animSettings.CompleteNormalizedTime)
            {
                _animSettings.Animator.Play(_animSettings.IdleAnimName);
                IsShowing = false;
            }

        }

        private void UpdateHideView()
        {
            if(!IsHiding)
                return;
            
            var stateInfo = _animSettings.Animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(_animSettings.HideAnimName) && stateInfo.normalizedTime >
                _animSettings.CompleteNormalizedTime)
            {
                _animSettings.Animator.Play(_animSettings.IdleAnimName);
                IsHiding = false;
            }
        }


        //model
        [Serializable]
        private class Settings
        {
            public Animator Animator;
            public float CompleteNormalizedTime = 0.95f; 

            [Space] 
            public string IdleAnimName = "Idle";
            public string ShowAnimName = "Show";
            public string HideAnimName = "Hide";
        }
    }
}