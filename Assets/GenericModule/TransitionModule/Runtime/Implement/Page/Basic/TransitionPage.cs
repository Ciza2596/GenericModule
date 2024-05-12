using CizaPageModule.Implement;
using UnityEngine;
using UnityEngine.UI;

namespace CizaTransitionModule.Implement
{
    public abstract class TransitionPage : Page
    {
        [SerializeField]
        private Image[] _refreshImages;


        protected void RefreshImages()
        {
            foreach (var refreshImage in _refreshImages)
            {
                refreshImage.gameObject.SetActive(false);
                refreshImage.gameObject.SetActive(true);
            }
        }
    }
}