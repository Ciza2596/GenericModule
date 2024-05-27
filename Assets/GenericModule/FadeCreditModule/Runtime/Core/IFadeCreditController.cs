using System;
using UnityEngine;

namespace CizaFadeCreditModule
{
    public interface IFadeCreditController
    {
        event Action OnShow;
        event Action OnHide;

        event Action OnComplete;

        Transform Pool { get; }
        Transform Content { get; }

        bool IsVisible { get; }
        bool IsHiding { get; }

        void Release();

        void Show();
        void Hide();

        void HideImmediately();
    }
}