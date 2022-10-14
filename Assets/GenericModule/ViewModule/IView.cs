using UnityEngine;


namespace ViewModule
{
    public interface IView
    {
        string Name { get; }
        GameObject GameObject { get; }
        bool IsShowing { get; }
        bool IsVisible { get; }
        bool IsHiding { get; }
        void Init();
        void Show(bool isImmediately);
        void Hide(bool isImmediately);
        void OnUpdate(float delta);
    }
}
