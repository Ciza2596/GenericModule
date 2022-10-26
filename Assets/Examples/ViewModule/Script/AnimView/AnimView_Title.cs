using UnityEngine;
using UnityEngine.UI;

namespace ViewModule.Example
{
    public class AnimView_Title : AnimViewBase
    {
        //private variable
        private IViewModule _viewModule;

        [SerializeField] private Button _goToLobby_Button;


        //baseView callback
        protected override void OnInit(params object[] items)
        {
            if (items is null || items.Length <= 0)
                return;

            if (items[0] is IViewModule viewModule)
                _viewModule = viewModule;


            _goToLobby_Button.onClick.AddListener(OnGoToLobbyButtonClick);
        }

        protected override void OnShow(params object[] items)
        {
            base.OnShow();
        }

        protected override void OnHide()
        {
            base.OnHide();
        }

        protected override void OnRelease() { }

        protected override void OnVisibleUpdateStart(float deltaTime) { }
        

        //private method
        private void OnGoToLobbyButtonClick()
        {
            _viewModule.HideView(ViewConfig.TITLE_NAME, () => _viewModule.ShowView(ViewConfig.LOBBY_NAME));
        }
    }
}