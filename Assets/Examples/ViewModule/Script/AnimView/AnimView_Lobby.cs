

using UnityEngine;
using UnityEngine.UI;

namespace ViewModule.Example
{
    public class AnimView_Lobby : AnimViewBase
    {
        
        //private variable
        private IViewModule _viewModule;

        [SerializeField] private Button _goToTitle_Button;
        
        //baseView callback
        protected override void OnInitStart(params object[] items)
        {
            if (items is null || items.Length <= 0)
                return;

            if (items[0] is IViewModule viewModule)
                _viewModule = viewModule;
            

            _goToTitle_Button.onClick.AddListener(OnGoToTitleButtonClick);
        }

        protected override void OnShowStart(params object[] items)
        {
            base.OnShowStart();
            
        }

        protected override void OnHideStart()
        {
            base.OnHideStart();
        }

        protected override void OnReleaseStart()
        {
        }

        protected override void OnVisibleUpdateStart(float deltaTime)
        {
        }

        //private method
        private void OnGoToTitleButtonClick()
        {
            _viewModule.HideView(ViewConfig.LOBBY_NAME);
            _viewModule.ShowView(ViewConfig.TITLE_NAME);
        }
    }
    
}
