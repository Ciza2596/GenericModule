using UnityEngine;

namespace CizaFadeCreditModule
{
    public interface IRow
    {
        int ViewOrder { get; }
        string Address { get; }


        bool IsNeedHiding => IsVisible && Time >= Duration;
        float Duration { get; }

        bool IsVisible { get; }

        bool IsShowing { get; }
        bool IsHiding { get; }

        float Time { get; }

        void Initialize(string address);
        void Release();

        void PlayEmpty(int viewOrder, Transform parent, Vector2 position, float duration, Vector2 size)
        {
            SetText(string.Empty);
            SetSprite(null);
            Play(viewOrder, parent, position, duration, size);
        }

        void PlayText(int viewOrder, Transform parent, Vector2 position, float duration, Vector2 size, string text)
        {
            SetText(text);
            SetSprite(null);
            Play(viewOrder, parent, position, duration, size);
        }

        void PlaySprite(int viewOrder, Transform parent, Vector2 position, float duration, Vector2 size, Sprite sprite)
        {
            SetText(string.Empty);
            SetSprite(sprite);
            Play(viewOrder, parent, position, duration, size);
        }

        void Play(int viewOrder, Transform parent, Vector2 position, float duration, Vector2 size)
        {
            SetViewOrder(viewOrder);
            SetParent(parent);
            SetPosition(position);
            SetDuration(duration);
            SetSize(size);
        }
        
        void SetTransformIndex(int index);

        void Close(Transform parent)
        {
            SetParent(parent);
            SetText(string.Empty);
            SetSprite(null);
            SetViewOrder(-1);
        }
        
        void SetViewOrder(int viewOrder);

        void SetParent(Transform parent);

        void SetPosition(Vector2 position);
        void SetDuration(float duration);

        void SetSize(float width, float height) =>
            SetSize(new Vector2(width, height));

        void SetSize(Vector2 size);

        void SetText(string text);
        void SetSprite(Sprite sprite);

        void Show();
        void Hide();
        void HideImmediately();

        void Tick(float deltaTime);
    }
}