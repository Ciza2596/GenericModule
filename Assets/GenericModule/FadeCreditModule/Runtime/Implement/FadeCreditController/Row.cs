using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace CizaFadeCreditModule.Implement
{
    public class Row : MonoBehaviour, IRow
    {
        [SerializeField]
        protected SettingImp Setting;

        public string Address { get; private set; }
        public float Duration { get; private set; }

        public bool IsVisible { get; private set; }
        public bool IsHiding { get; private set; }

        public float Time { get; private set; }


        public void Release() =>
            DestroyImmediate(gameObject);

        public void Initialize(string address) =>
            Address = address;

        public void SetParent(Transform transform) =>
            transform.SetParent(transform);

        public void SetPosition(Vector2 position) =>
            Setting.SetPosition(position);

        public virtual void SetDuration(float duration) =>
            Duration = duration;

        public void SetSize(Vector2 size) =>
            Setting.SetSize(size);

        public void SetText(string text) =>
            Setting.SetText(text);

        public void SetSprite(Sprite sprite) =>
            Setting.SetSprite(sprite);

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

        public virtual void HideImmediately()
        {
            Setting.HideImmediately();
            IsVisible = false;
            IsHiding = false;
        }

        public virtual void Tick(float deltaTime) =>
            Time += deltaTime;


        [Serializable]
        public class SettingImp
        {
            [SerializeField]
            private RectTransform _rectTransform;

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

            [Space]
            [SerializeField]
            private TMP_Text _text;

            [SerializeField]
            private Image _image;


            public RectTransform RectTransform => _rectTransform;

            public Animator Animator => _animator;

            public void SetPosition(Vector2 position) =>
                RectTransform.anchoredPosition = position;

            public void SetSize(Vector2 size) =>
                RectTransform.sizeDelta = size;

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

            public void SetText(string text) =>
                _text.text = text;

            public void SetSprite(Sprite sprite)
            {
                _image.enabled = sprite != null;
                _image.sprite = sprite;
            }
        }
    }
}