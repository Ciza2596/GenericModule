using System;
using CizaPageModule.Implement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CizaPageModule.Example1
{
    public class LobbyPage : Page, IShowingAnimated, IShowingComplete, IHidingStart
    {
        //private variable
        [SerializeField] private Button _titleButton;
        
        [SerializeField] private BaseAnimPlayer _showingAnimPlayer;
        
        private PageModule _pageModule;

        
        //unity callback
        private void Awake()
        {
            var pageModuleExample = FindObjectOfType<PageModuleExample>();
            _pageModule = pageModuleExample.PageModule;
        }

        
        //PageModule callback
        public UniTask PlayShowingAnimation() => _showingAnimPlayer.Play();
        public void OnShowingComplete() => _titleButton.onClick.AddListener(OnTitleButtonClick);

        public void OnHidingStart() => _titleButton.onClick.RemoveListener(OnTitleButtonClick);


        //private method
        private void OnTitleButtonClick() =>
            _pageModule.Hide<LobbyPage>(() => _pageModule.Show<TitlePage>());
        
    }
}