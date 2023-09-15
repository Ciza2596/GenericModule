
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaPageModule.Example1
{
    public class AnimatorPlayer : BaseAnimPlayer
    {
        [Space] [SerializeField] private string _playStateName = "Play";
        [SerializeField] private float _normalizedTime = 0.95f;
        [SerializeField] private string _idleStateName = "Idle";

        [Space] [SerializeField] private Animator _animator;


    
        public override async UniTask Play()
        {
            _animator.Play(_idleStateName);
            await UniTask.Yield();

            _animator.Play(_playStateName);
            await UniTask.Yield();

            while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _normalizedTime)
                await UniTask.Yield();
        }
    }
}