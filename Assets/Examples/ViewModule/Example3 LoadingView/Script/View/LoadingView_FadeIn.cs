using UnityEngine;

namespace ViewModule.Example3
{
    public class LoadingView_FadeIn : BaseTransitionInView
    {
        [SerializeField] private string _animFadingStateName = "Fading";
        [SerializeField] private string _animIdleStateName = "Idle";
        [SerializeField] private Animator _animator;
        
        private bool _isPlayingFadingAnim;

        protected override void OnShow(params object[] parameters)
        {
            base.OnShow(parameters);
            
            _animator.Play(_animFadingStateName);
            _animator.Update(Time.deltaTime);
            
            _isPlayingFadingAnim = true;
        }

        protected override void OnVisibleTick(float deltaTime)
        {
            base.OnVisibleTick(deltaTime);
            
            if(!_isPlayingFadingAnim)
                return;
            
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            var isPlaying = stateInfo.IsName(_animIdleStateName);
            if (isPlaying)
            {
                _isPlayingFadingAnim = false;
                _hasCompleted = true;
            }
        }
    }
}
