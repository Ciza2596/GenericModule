using UnityEngine;
using UnityEngine.UI;

namespace ViewModule.Example1
{
    public class View_Title : BaseView
    {
        //private variable
        private ViewModule _viewModule;

        [SerializeField] private Button _goToLobby_Button;


        //baseView callback
        protected override void OnInit(params object[] parameters)
        {
            if (parameters is null || parameters.Length <= 0)
                return;

            if (parameters[0] is ViewModule viewModule)
                _viewModule = viewModule;


            _goToLobby_Button.onClick.AddListener(OnGoToLobbyButtonClick);
        }

        protected override void OnShow(params object[] parameters)
        {
            base.OnShow();
        }

        protected override void OnHide()
        {
            base.OnHide();
        }
        

        protected override void OnRelease() { }

        protected override void OnVisibleTick(float deltaTime) { }

        protected override void OnTick(float deltaTime) { }

        //private method
        private void OnGoToLobbyButtonClick()
        {
            _viewModule.HideView(ViewTypes.Title.ToString(), () => _viewModule.ShowView(ViewTypes.Lobby.ToString()));
        }
    }
}