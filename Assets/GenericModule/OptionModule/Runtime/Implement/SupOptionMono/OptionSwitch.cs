using System;
using CizaCore.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaOptionModule.Implement
{
    public class OptionSwitch : OptionSup
    {
        [SerializeField]
        private Switch _switch;

        public event Action<bool> OnIsOnChanged;

        public bool IsOn => _switch.IsOn;

        public void SetIsOn(bool isOn)
        {
            if (isOn)
                _switch.TurnOn();
            else
                _switch.TurnOff();
        }

        public override void Initialize(Option option)
        {
            base.Initialize(option);

            Assert.IsNotNull(_switch, $"[OptionSwitch::Initialize] Option: {name} is not set switch.");
            _switch.OnIsOnChanged += OnIsOnChangedImp;
        }

        public override void Release(Option option)
        {
            base.Release(option);
            _switch.OnIsOnChanged -= OnIsOnChangedImp;
        }

        private void OnIsOnChangedImp(bool isOn) =>
            OnIsOnChanged?.Invoke(isOn);
    }
}