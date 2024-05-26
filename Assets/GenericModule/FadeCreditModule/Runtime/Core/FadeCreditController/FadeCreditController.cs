using System;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace CizaFadeCreditModule
{
    public class FadeCreditController : MonoBehaviour, IFadeCreditController
    {
        [SerializeField]
        protected SettingImp Setting;

        public Transform Pool => Setting.Pool;

        public Transform Content => Setting.Content;

        public bool IsVisible { get; private set; }
        public bool IsHiding { get; private set; }

        public void Release() =>
            DestroyImmediate(gameObject);

        public virtual async void Show()
        {
            IsVisible = true;
            await Setting.ShowAsync();
        }

        public virtual async void Hide()
        {
            IsHiding = true;
            await Setting.HideAsync();
            HideImmediately();
        }

        public void HideImmediately()
        {
            Setting.HideImmediately();
            IsVisible = false;
            IsHiding = false;
        }

        [Serializable]
        public class SettingImp
        {
            [SerializeField]
            private Transform _pool;

            [SerializeField]
            private Transform _content;

            [SerializeField]
            private CanvasGroup _canvasGroup;

            [Space]
            [SerializeField]
            private string _showStateName = "Show";

            [SerializeField]
            private string _hideStateName = "Hide";

            [Space]
            [SerializeField]
            private Animator _animator;


            public Transform Pool => _pool;
            public Transform Content => _content;
            public Animator Animator => _animator;


            public UniTask ShowAsync()
            {
                _canvasGroup.alpha = 1;
                return Animator.PlayAtStartAsync(_showStateName, 0, true, null);
            }

            public async UniTask HideAsync()
            {
                await Animator.PlayAtStartAsync(_hideStateName, 0, true, null);
                HideImmediately();
            }

            public void HideImmediately() =>
                _canvasGroup.alpha = 0;
        }
    }
}