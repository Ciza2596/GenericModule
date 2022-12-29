using System;
using UnityEngine;

namespace SceneModule.Example1
{
    public abstract class BaseTransitionView : MonoBehaviour, ITransitionView
    {
        protected Action _onComplete;
        
        public virtual void Play(Action onComplete)
        {
            gameObject.SetActive(true);
            _onComplete = onComplete;
        }
    }
}
