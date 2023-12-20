using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace CizaOptionModule.Implement
{
    public class OptionSlider : MonoBehaviour
    {
        [SerializeField]
        private Slider _slider;

        public Slider Slider
        {
            get
            {
                Assert.IsNotNull(_slider, $"[OptionSlider::Slider] Option: {name} is not set slider.");
                return _slider;
            }
        }
    }
}