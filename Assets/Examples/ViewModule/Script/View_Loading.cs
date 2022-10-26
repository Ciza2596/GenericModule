using UnityEngine;
using UnityEngine.UI;

namespace ViewModule.Example
{
    public class View_Loading : ViewBase
    {

        //private variable
        private ViewModule _viewModule;
        
        
        private string _nextViewName;
        
        //baseView callback
        protected override void OnInit(params object[] items)
        {
            if (items is null || items.Length <= 0)
                return;
            
            if (items[0] is ViewModule viewModule)
                _viewModule = viewModule;
        }

        protected override void OnShow(params object[] items)
        {
            base.OnShow();
            
            if (items[0] is string nextViewName)
                _nextViewName = nextViewName;
        }

        protected override void OnHide()
        {
            base.OnHide();
        }
        

        protected override void OnRelease() { }

        protected override void OnVisibleUpdateStart(float deltaTime) { }

        protected override void OnUpdateStart(float deltaTime) { }

        //private method
    }
}