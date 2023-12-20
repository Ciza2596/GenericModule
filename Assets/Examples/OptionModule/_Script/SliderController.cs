using System;
using UnityEngine;
using UnityEngine.UI;

namespace CizaInputModule.Example
{
    public class SliderController : MonoBehaviour
    {
        [SerializeField]
        private Slider _slider;

        // value
        public event Action<float> OnValueChanged;

        public float Value => _slider.value;

        public void SetValue(float value, bool isTriggerCallback)
        {
            if (isTriggerCallback)
                _slider.value = value;
            else
                _slider.SetValueWithoutNotify(value);
        }


        private void Awake()
        {
            _slider.onValueChanged.AddListener(OnValueChangedImp);
        }

        private void OnValueChangedImp(float value)
        {
            OnValueChanged?.Invoke(value);
        }
    }
}