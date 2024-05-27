using System;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace CizaFadeCreditModule
{
    public class FadeCreditController : MonoBehaviour, IFadeCreditController
    {
        [SerializeField]
        protected SettingImp Setting;

        public event Action OnShow;
        public event Action OnHide;
        public event Action OnComplete;

        public Transform Pool => Setting.Pool;

        public Transform Content => Setting.Content;

        public bool IsVisible { get; private set; }
        public bool IsHiding { get; private set; }

        public virtual async void Show()
        {
            IsVisible = true;
            Setting.EnableCamera();
            await Setting.ShowAsync();
            OnShow?.Invoke();
        }

        public virtual async void Hide()
        {
            IsHiding = true;
            OnHide?.Invoke();
            await Setting.HideAsync();
            HideEnd();
        }

        public void HideImmediately()
        {
            OnHide?.Invoke();
            HideEnd();
        }

        public void Tick(float deltaTime) =>
            Setting.Tick(deltaTime);

        private void HideEnd()
        {
            Setting.HideImmediately();
            IsVisible = false;
            IsHiding = false;
            OnComplete?.Invoke();
            Setting.DisableCamera();
        }

        [Serializable]
        public class SettingImp
        {
            [SerializeField]
            private Camera _camera;

            [Space]
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


            public Camera Camera => _camera;

            public Transform Pool => _pool;
            public Transform Content => _content;
            public Animator Animator => _animator;


            public void EnableCamera() =>
                Camera.enabled = true;

            public void DisableCamera() =>
                Camera.enabled = false;

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

            public void Tick(float deltaTime) =>
                _animator.Update(deltaTime);

            public void HideImmediately() =>
                _canvasGroup.alpha = 0;
        }
    }
}