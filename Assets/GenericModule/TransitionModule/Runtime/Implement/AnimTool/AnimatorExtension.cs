using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaTransitionModule.Implement
{
    public static class AnimatorExtension
    {
        public static async void PlayAtStart(this Animator animator, string stateName, int layer, Action onPlay) =>
            await animator.PlayAtStartAsync(stateName, layer, onPlay);
        
        
        public static async UniTask PlayAtStartAsync(this Animator animator, string stateName, int layer, Action onPlay)
        {
            try
            {
                animator.Play(stateName, layer, 0);
                animator.Update(0);
                onPlay?.Invoke();
                while (animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 0.95f)
                    await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
}