using UnityEngine;


namespace ViewModule
{
    public interface IView
    {
        GameObject GameObject { get; }
        bool IsShowing { get; }
        bool IsHiding { get; }
        void Init(params object[] parameters);    // load 時，跑一遍。
        void Show(params object[] parameters);    // show 時，會跑一遍。
        void Hide();    // load, release, hide 時，會跑一遍。
        void HideAfter();
        void Release(); // release 時，會跑一遍。
        void Tick(float deltaTime);        //Show的當下就會update
    }
}
