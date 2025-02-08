using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace CizaOptionModule.Implement
{
    public class OptionSlider : OptionSup
    {
        [SerializeField]
        private Slider _slider;

        public event Action<float> OnValueChanged;

        public float Value => _slider.value;

        public void SetValue(float value, bool isTriggerCallback)
        {
            if (isTriggerCallback)
                _slider.value = value;
            else
                _slider.SetValueWithoutNotify(value);
        }

        public override void Initialize(Option option)
        {
            base.Initialize(option);

            Assert.IsNotNull(_slider, $"[OptionSlider::Initialize] Option: {name} is not set slider.");
            Option.OnSetIsUnlock += OnSetIsUnlockImp;
            _slider.onValueChanged.AddListener(OnValueChangedImp);
        }

        public override void Release(Option option)
        {
            base.Release(option);
            _slider.onValueChanged.RemoveListener(OnValueChangedImp);
            Option.OnSetIsUnlock -= OnSetIsUnlockImp;
        }

        private void OnValueChangedImp(float value) =>
            OnValueChanged?.Invoke(value);

        private void OnSetIsUnlockImp(string optionKey, bool isUnlock)
        {
            if (optionKey != Option.Key)
                return;

            _slider.interactable = isUnlock;
        }
    }
}