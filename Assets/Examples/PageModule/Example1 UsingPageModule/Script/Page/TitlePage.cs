using Cysharp.Threading.Tasks;
using PageModule.Implement;
using UnityEngine;
using UnityEngine.UI;

namespace PageModule.Example1
{
    public class TitlePage: BasePage, IShowingAnimated, IShowingComplete, IHidingStart
    {
        //private variable
        [SerializeField]
        private Button _lobbyButton;

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
        public void OnShowingComplete() => _lobbyButton.onClick.AddListener(OnLobbyButtonClick);
        
        public void OnHidingStart() => _lobbyButton.onClick.RemoveListener(OnLobbyButtonClick);


        //private method
        private void OnLobbyButtonClick() =>
            _pageModule.Hide<TitlePage>(() => _pageModule.Show<LobbyPage>());
    }
}