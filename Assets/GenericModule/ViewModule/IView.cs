using UnityEngine;


namespace ViewModule
{
    public interface IView
    {
        GameObject GameObject { get; }
        bool IsShowing { get; }
        bool IsHiding { get; }
        void Init();    // load 時，跑一遍。
        void Show();    // show 時，會跑一遍。
        void Hide();    // load, release, hide 時，會跑一遍。
        void Release(); // release 時，會跑一遍。
        void OnUpdate(float deltaTime);  //只有在顯示時才會跑。
    }
}
