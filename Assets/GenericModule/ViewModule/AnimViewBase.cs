
using System;
using UnityEngine;

namespace ViewModule
{
    public abstract class AnimViewBase : ViewBase
    {
        //private variable
        [SerializeField] private Settings _animSettings;

        private bool _isPlayingShowAnim;
        private bool _isPlayingHideAnim;

        
        //viewBase callback
        protected override void OnUpdateStart(float deltaTime)
        {
            UpdateShowView();
            UpdateHideView();
        }


        //protected method
        protected override void OnShow(params object[] items)
        { 
            _animSettings.Animator.Play(_animSettings.ShowAnimName);
            _isPlayingShowAnim = true;
        }

        protected override void OnHide()
        {
            _animSettings.Animator.Play(_animSettings.HideAnimName);
            _isPlayingHideAnim = true;
        }

        //private method
        private void UpdateShowView()
        {
            if(!IsShowing || !_isPlayingShowAnim)
                return;

            var stateInfo = _animSettings.Animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(_animSettings.ShowedAnimName))
            {
                IsShowing = false;
                _isPlayingShowAnim = false;
            }

        }

        private void UpdateHideView()
        {
            if(!IsHiding || !_isPlayingHideAnim)
                return;
            
            var stateInfo = _animSettings.Animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(_animSettings.HidedAnimName))
            {
                IsHiding = false;
                _isPlayingHideAnim = false;
            }
        }


        //model
        [Serializable]
        private class Settings
        {
            public Animator Animator;

            [Space]
            
            public string ShowAnimName = "Show";
            public string ShowedAnimName = "Showed";
            public string HideAnimName = "Hide";
            public string HidedAnimName = "Hided";
        }
    }
}