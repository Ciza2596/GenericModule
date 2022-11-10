
namespace ViewModule
{
    public abstract class TransitionOutViewBase : ViewBase
    {
        //private variable
        private ViewModule _viewModule;

        private string _selfViewName;

        private bool _once;
        
        //protected variable
        protected bool _hasCompleted;


        //baseView callback
        protected override void OnInit(params object[] parameters)
        {
            if (parameters is null || parameters.Length <= 0)
                return;

            if (parameters[0] is ViewModule viewModule)
                _viewModule = viewModule;
        }

        protected override void OnShow(params object[] parameters)
        {
            base.OnShow(parameters);

            _once = true;
            _hasCompleted = false;
            
            
            if(parameters.Length < 1)
                return;
            
            if (parameters[0] is string selfViewName)
                _selfViewName = selfViewName;
        }

        protected override void OnHide()
        {
            base.OnHide();
        }


        protected override void OnRelease() { }

        protected override void OnVisibleTick(float deltaTime)
        {
            if (!_once || !_hasCompleted)
                return;

            _once = false;
            
            _viewModule.HideView(_selfViewName);
        }

        protected override void OnTick(float deltaTime) { }
    }
}