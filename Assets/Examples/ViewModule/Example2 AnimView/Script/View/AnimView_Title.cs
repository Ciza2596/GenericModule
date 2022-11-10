using UnityEngine;
using UnityEngine.UI;

namespace ViewModule.Example2
{
    public class AnimView_Title : BaseAnimView
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
            _viewModule.HideView(AnimViewTypes.Title.ToString(), () => _viewModule.ShowView(AnimViewTypes.Lobby.ToString()));
        }
    }
}