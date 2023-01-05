using System.Threading.Tasks;
using UnityEngine;


public class AnimatorPlayer : BaseAnimPlayer
{
    [Space] [SerializeField] private string _playStateName = "Play";
    [SerializeField] private float _normalizedTime = 0.95f;
    [SerializeField] private string _idleStateName = "Idle";

    [Space] [SerializeField] private Animator _animator;


    public override async Task Play()
    {
        _animator.Play(_idleStateName);
        await Task.Yield();

        _animator.Play(_playStateName);
        await Task.Yield();

        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _normalizedTime)
            await Task.Yield();
    }
}