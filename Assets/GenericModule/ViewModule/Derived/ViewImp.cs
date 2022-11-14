using UnityEngine;

namespace ViewModule.Derived
{
    public class ViewImp : MonoBehaviour, IView
    {
        //private variable
        private object[] _initParameters;
        private object[] _showParameters;
        
        //public variable
        public GameObject GameObject => gameObject;
        public bool IsShowing { get; private set; }

        public bool IsHiding { get; private set; }

        public void Init(params object[] parameters)
        {
            _initParameters = parameters;
        }

        public void Show(params object[] parameters)
        {
            _showParameters = parameters;
        }

        public void Hide()
        {
        }

        public void CompleteHiding()
        {
        }

        public void Release()
        {
        }

        public void Tick(float deltaTime)
        {
        }

        //for test
        public object[] GetInitParameters() => _initParameters;
        public object[] GetShowParameters() => _showParameters;

        public void SetIsShowing(bool isShowing) => IsHiding = isShowing;
        
        public void SetIsHiding(bool isHiding) =>
            IsHiding = isHiding;
    }
}