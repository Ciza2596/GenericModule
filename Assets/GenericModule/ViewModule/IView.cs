using UnityEngine;


namespace ViewModule
{
    public interface IView
    {
        GameObject GameObject { get; }
        bool IsShowing { get; }
        bool IsHiding { get; }
        void Init(params object[] items);    // load 時，跑一遍。
        void Show(params object[] items);    // show 時，會跑一遍。
        void Hide();    // load, release, hide 時，會跑一遍。
        void HideAfter();
        void Release(); // release 時，會跑一遍。
        void OnVisibleUpdate(float deltaTime);  //只有在顯示時才會跑。
        void OnUpdate(float deltaTime);        //會一直更新
    }
}
