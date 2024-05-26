using UnityEngine;

namespace CizaFadeCreditModule
{
    public interface IRow
    {
        string Address { get; }


        bool IsNeedHiding => Time >= Duration;
        float Duration { get; }

        bool IsVisible { get; }
        bool IsHiding { get; }
        
        float Time { get; }

        void Initialize(string address);
        void Release();

        void PlayEmpty(Transform parent, Vector2 position, float duration, Vector2 size)
        {
            SetText(string.Empty);
            SetSprite(null);
            Play(parent, position, duration, size);
        }

        void PlayText(Transform parent, Vector2 position, float duration, Vector2 size, string text)
        {
            SetText(text);
            SetSprite(null);
            Play(parent, position, duration, size);
        }

        void PlaySprite(Transform parent, Vector2 position, float duration, Vector2 size, Sprite sprite)
        {
            SetText(string.Empty);
            SetSprite(sprite);
            Play(parent, position, duration, size);
        }

        void Play(Transform parent, Vector2 position, float duration, Vector2 size)
        {
            SetParent(parent);
            SetPosition(position);
            SetDuration(duration);
            SetSize(size);
        }

        void Close(Transform parent)
        {
            SetParent(parent);
        }


        void SetParent(Transform transform);

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