using UnityEngine;
using UnityEngine.UI;

namespace ViewModule.Example3
{
    public class FadeView_Title : ViewBase
    {
        //private variable
        private ViewModule _viewModule;

        [SerializeField] private Button _goToLobby_Button;


        //baseView callback
        protected override void OnInit(params object[] items)
        {
            if (items is null || items.Length <= 0)
                return;

            if (items[0] is ViewModule viewModule)
                _viewModule = viewModule;


            _goToLobby_Button.onClick.AddListener(OnGoToLobbyButtonClick);
        }

        protected override void OnShow(params object[] items)
        {
            base.OnShow(items);
        }

        protected override void OnHide()
        {
            base.OnHide();
        }

        protected override void OnRelease()
        {
        }

        protected override void OnVisibleUpdateStart(float deltaTime)
        {
        }


        //private method
        private void OnGoToLobbyButtonClick()
        {
            _viewModule.HideViewThenFadeShow(FadeViewTypes.FadeIn.ToString(), FadeViewTypes.Title.ToString(),
                FadeViewTypes.Loading.ToString(), FadeViewTypes.FadeOut.ToString(), FadeViewTypes.Lobby.ToString());
        }
    }
}