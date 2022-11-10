

using UnityEngine;
using UnityEngine.UI;

namespace ViewModule.Example2
{
    public class AnimView_Lobby : BaseAnimView
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
            _viewModule.HideView(AnimViewTypes.Lobby.ToString(), () => _viewModule.ShowView(AnimViewTypes.Title.ToString()));
        }
    }
    
}
