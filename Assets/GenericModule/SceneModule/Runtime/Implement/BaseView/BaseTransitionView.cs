using System;
using UnityEngine;

namespace SceneModule.Implement
{
    public abstract class BaseTransitionView : MonoBehaviour, ITransitionView
    {
        protected Action _onComplete;
        
        public virtual void Play(Action onComplete) =>
            _onComplete = onComplete;
    }
}
