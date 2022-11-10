
namespace ViewModule
{
    public class BaseLoadingView : BaseView
    {
        //private variable
        private object[] _parameters;

        private string _selfViewName;

        private string _nextTransitionOutViewName;
        private string _nextViewName;

        private ViewModule _viewModule;

        private bool _once;

        //protected variable
        protected bool _isCompleting;


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

            if (parameters[0] is string selfViewName)
                _selfViewName = selfViewName;
            
            if (parameters[1] is string nextViewName)
                _nextViewName = nextViewName;
            
            if (parameters[2] is string nextTransitionOutName)
                _nextTransitionOutViewName = nextTransitionOutName;
            

            var length = parameters.Length;
            _parameters = new object[length - 3];

            if (length > 3)
                for (var i = 3; i < length; i++)
                    parameters[i] = _parameters[i];

            _once = true;
            _isCompleting = false;
        }

        protected override void OnHide()
        {
            base.OnHide();
        }


        protected override void OnRelease()
        {
        }

        protected override void OnVisibleTick(float deltaTime)
        {
            if (!_once || !_isCompleting)
                return;

            _once = false;

            _viewModule.HideView(_selfViewName,
                () => _viewModule.ShowViewWithTransition(
                    _nextViewName, _nextTransitionOutViewName, _parameters));
        }

        protected override void OnTick(float deltaTime)
        {
        }
    }
}