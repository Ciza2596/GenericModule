using CizaPageModule.Implement;
using UnityEngine;

namespace CizaTransitionModule.Implement
{
    public abstract class TransitionPage : Page
    {
        protected void RefreshUI()
        {
            Canvas.ForceUpdateCanvases();
        }
    }
}