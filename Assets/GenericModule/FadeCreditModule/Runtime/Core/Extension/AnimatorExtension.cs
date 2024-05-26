using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaFadeCreditModule
{
    public static class AnimatorExtension
    {
        public static async void PlayAtStart(this Animator animator, string stateName, int layer, bool isStop, Action onPlay) =>
            await animator.PlayAtStartAsync(stateName, layer, isStop, onPlay);


        public static async UniTask PlayAtStartAsync(this Animator animator, string stateName, int layer, bool isStop, Action onPlay)
        {
            try
            {
                animator.speed = 1;
                animator.Play(stateName, layer, 0);
                animator.Update(0);
                onPlay?.Invoke();
                while (animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 0.95f)
                    await UniTask.Yield(PlayerLoopTiming.PreLateUpdate);

                if (isStop)
                    animator.speed = 0;
            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
}