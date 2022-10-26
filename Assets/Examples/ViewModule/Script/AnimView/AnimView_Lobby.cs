

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
        protected override void OnInit(params object[] items)
        {
            if (items is null || items.Length <= 0)
                return;

            if (items[0] is IViewModule viewModule)
                _viewModule = viewModule;
            

            _goToTitle_Button.onClick.AddListener(OnGoToTitleButtonClick);
        }

        protected override void OnShow(params object[] items)
        {
            base.OnShow();
            
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
        private void OnGoToTitleButtonClick()
        {
            _viewModule.HideView(ViewConfig.LOBBY_NAME, () => _viewModule.ShowView(ViewConfig.TITLE_NAME));
        }
    }
    
}
