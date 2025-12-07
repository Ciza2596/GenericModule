using System;
using CizaUniTask;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CizaFadeCreditModule.Implement
{
    public class Row : MonoBehaviour, IRow
    {
        [SerializeField]
        protected SettingImp Setting;

        public int ViewOrder { get; private set; }
        public string Address { get; private set; }

        public float Duration { get; private set; }

        public bool IsVisible { get; private set; }

        public bool IsShowing { get; private set; }
        public bool IsHiding { get; private set; }

        public float Time { get; private set; }


        public virtual void Release() =>
            DestroyImmediate(gameObject);

        public virtual void Initialize(string address) =>
            Address = address;

        public void SetTransformIndex(int index) =>
            transform.SetSiblingIndex(index);


        public virtual void SetViewOrder(int viewOrder) =>
            ViewOrder = viewOrder;

        public virtual void SetParent(Transform parent) =>
            transform.SetParent(parent);

        public virtual void SetPosition(Vector2 position) =>
            Setting.SetPosition(position);

        public virtual void SetDuration(float duration) =>
            Duration = duration;

        public virtual void SetSize(Vector2 size) =>
            Setting.SetSize(size);

        public virtual void SetText(string text) =>
            Setting.SetText(text);

        public virtual void SetSprite(Sprite sprite) =>
            Setting.SetSprite(sprite);

        public virtual async void Show()
        {
            IsVisible = true;
            IsShowing = true;
            Time = 0;
            gameObject.SetActive(true);
            await Setting.ShowAsync();
            IsShowing = false;
        }

        public virtual async void Hide()
        {
            IsHiding = true;
            await Setting.HideAsync();
            HideImmediately();
        }

        public virtual void HideImmediately()
        {
            try
            {
                gameObject.SetActive(false);
                IsVisible = false;
                IsHiding = false;
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        public virtual void Tick(float deltaTime)
        {
            Time += deltaTime;
            Setting.Tick(deltaTime);
        }


        [Serializable]
        public class SettingImp
        {
            [SerializeField]
            private RectTransform _rectTransform;

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

            public UniTask ShowAsync() =>
                Animator.PlayAtStartAsync(_showStateName, 0, true, null);

            public UniTask HideAsync() =>
                Animator.PlayAtStartAsync(_hideStateName, 0, true, null);

            public void Tick(float deltaTime) =>
                _animator.Update(deltaTime);

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