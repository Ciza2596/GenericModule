using UnityEngine;

namespace CizaFadeCreditModule
{
    public interface IFadeCreditController
    {

        Transform Pool { get; }
        Transform Content { get; }

        bool IsVisible { get; }
        bool IsHiding { get; }

        void Show();
        void Hide();

        void HideImmediately();
    }
}