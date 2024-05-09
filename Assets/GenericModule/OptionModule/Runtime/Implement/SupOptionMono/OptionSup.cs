using UnityEngine;
using UnityEngine.Assertions;

namespace CizaOptionModule.Implement
{
    public abstract class OptionSup : MonoBehaviour, IOptionSup
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        protected Option Option { get; private set; }

        public bool IsInitialized { get; private set; }

        public virtual void Initialize(Option option)
        {
            IsInitialized = true;

            Option = option;
            Assert.IsNotNull(Option, $"[{GetType().Name}::Awake] Option should be found.");
        }

        public virtual void Release(Option option)
        {
            IsInitialized = false;
        }

        public void Show()
        {
            _canvasGroup.alpha = 1;
            EnableInteractable();
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            DisableInteractable();
        }

        public void EnableInteractable()
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public void DisableInteractable()
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}