using UnityEngine;
using UnityEngine.UI;

namespace ViewModule.Example3
{
    public class LoadingView_Lobby : BaseView
    {
        //private variable
        private ViewModule _viewModule;

        [SerializeField] private Button _goToTitle_Button;

        //baseView callback
        protected override void OnInit(params object[] parameters)
        {
            if (parameters is null || parameters.Length <= 0)
                return;

            if (parameters[0] is ViewModule viewModule)
                _viewModule = viewModule;


            _goToTitle_Button.onClick.AddListener(OnGoToTitleButtonClick);
        }
        

        //private method
        private void OnGoToTitleButtonClick()
        {
            _viewModule.ChangeView(FadeViewTypes.Lobby.ToString(), FadeViewTypes.FadeIn.ToString(),
                FadeViewTypes.Loading.ToString(), FadeViewTypes.Title.ToString(), FadeViewTypes.FadeOut.ToString());
        }
    }
}