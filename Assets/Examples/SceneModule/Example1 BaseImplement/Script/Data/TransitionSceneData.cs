using System;
using UnityEngine;

namespace SceneModule.Example1
{
    [Serializable]
    public class TransitionSceneData
    {
        [SerializeField]
        private string _transitionInViewName;

        [SerializeField]
        private string _currentSceneName;

        [Space]
        [SerializeField]
        private string _loadingViewName;

        [Space]
        [SerializeField]
        private string _transitionOutViewName;
        
        [SerializeField]
        private string _nextSceneName;


        public string TransitionInViewName => _transitionInViewName;
        public string CurrentSceneName => _currentSceneName;

        
        public string LoadingViewName => _loadingViewName;


        public string TransitionOutViewName =>_transitionOutViewName;
        public string NextSceneName => _nextSceneName;
    }
}
