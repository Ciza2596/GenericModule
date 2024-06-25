using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace CizaOptionModule.Implement
{
    public class OptionButton : OptionSup
    {
        [SerializeField]
        private Button _button;

        public event Action OnClick;

        public override void Initialize(Option option)
        {
            base.Initialize(option);

            Assert.IsNotNull(_button, $"[OptionButton::Initialize] Option: {name} is not set button.");
            Option.OnSetIsUnlock += OnSetIsUnlockImp;
            _button.onClick.AddListener(OnClickImp);
        }

        public override void Release(Option option)
        {
            base.Release(option);
            _button.onClick.RemoveListener(OnClickImp);
            Option.OnSetIsUnlock -= OnSetIsUnlockImp;
        }


        public void Click() =>
            _button.onClick?.Invoke();

        private void OnClickImp() =>
            OnClick?.Invoke();
        
        private void OnSetIsUnlockImp(string optionKey, bool isUnlock)
        {
            if (optionKey != Option.Key)
                return;

            _button.enabled = isUnlock;
        }
    }
}