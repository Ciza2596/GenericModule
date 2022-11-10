using UnityEngine;
using UnityEngine.UI;

namespace ViewModule.Example3
{
    public class LoadingView_Title : BaseView
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


        //private method
        private void OnGoToLobbyButtonClick()
        {
            _viewModule.ChangeView(FadeViewTypes.Title.ToString(), FadeViewTypes.FadeIn.ToString(),
                FadeViewTypes.Loading.ToString(), FadeViewTypes.Lobby.ToString(), FadeViewTypes.FadeOut.ToString());
        }
    }
}