using UnityEngine;


namespace ViewModule
{
    public interface IView
    {
        GameObject GameObject { get; }
        bool IsShowing { get; }
        bool IsHiding { get; }
        void Init(params object[] parameters); 
        void Show(params object[] parameters);
        void Hide();
        void CompleteHiding();
        void Release();
        void Tick(float deltaTime);
    }
}
