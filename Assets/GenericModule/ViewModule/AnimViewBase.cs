
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
            _animSettings.Animator.Play(_animSettings.ShowedAnimName);
        }

        protected override void OnHideStart()
        {
            _animSettings.Animator.Play(_animSettings.HidedAnimName);
        }

        //private method
        private void UpdateShowView()
        {
            if(!IsShowing)
                return;

            var stateInfo = _animSettings.Animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(_animSettings.ShowedAnimName))
            {
                IsShowing = false;
            }

        }

        private void UpdateHideView()
        {
            if(!IsHiding)
                return;
            
            var stateInfo = _animSettings.Animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(_animSettings.HidedAnimName))
            {
                IsHiding = false;
            }
        }


        //model
        [Serializable]
        private class Settings
        {
            public Animator Animator;

            [Space] 
            public string ShowedAnimName = "Showed";
            public string HidedAnimName = "Hided";
        }
    }
}