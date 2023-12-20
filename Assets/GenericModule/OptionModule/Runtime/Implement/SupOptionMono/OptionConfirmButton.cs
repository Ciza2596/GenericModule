using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace CizaOptionModule.Implement
{
    public class OptionConfirmButton : OptionSup
    {
        [SerializeField]
        private Button _button;

        public override void Initialize(Option option)
        {
            base.Initialize(option);

            _button.onClick.AddListener(OnClick);
            Assert.IsNotNull(_button, $"[{GetType().Name}::Awake] Button is not referenced.");
        }

        public override void Release(Option option)
        {
            base.Release(option);
            _button.onClick.RemoveListener(OnClick);
        }

        protected void OnClick() =>
            Option.TryConfirm();
    }
}