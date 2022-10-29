
using UnityEngine;

namespace ViewModule.Example
{
    public class FadeView_FadeOut : FadeOutViewBase
    {
        [SerializeField] private string _animFadingStateName = "Fading";
        [SerializeField] private string _animIdleStateName = "Idle";
        [SerializeField]
        private Animator _animator;
        
        private bool _isPlayingFadingAnim;

        protected override void OnShow(params object[] items)
        {
            base.OnShow(items);
            
            _animator.Play(_animFadingStateName);
            _isPlayingFadingAnim = true;
        }

        protected override void OnVisibleUpdateStart(float deltaTime)
        {
            base.OnVisibleUpdateStart(deltaTime);
            
            if(!_isPlayingFadingAnim)
                return;
            
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(_animIdleStateName))
            {
                _isPlayingFadingAnim = false;
                _isCompletedFadeOut = true;
            }
        }
    }
}
