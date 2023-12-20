using UnityEngine;

namespace CizaInputModule.Example
{
    public class SliderControllerExample : MonoBehaviour
    {
        [SerializeField]
        private SliderController _sliderController;

        [SerializeField]
        private float _defaultValue;

        [SerializeField]
        private float _currentValue;

        private void Awake()
        {
            _currentValue = _defaultValue;
        }


        private void Update()
        {
            if (_sliderController.Value != _currentValue)
                _sliderController.SetValue(_currentValue, true);
        }
    }
}