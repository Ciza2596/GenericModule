using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace CizaOptionModule.Implement
{
    public class OptionButton : MonoBehaviour
    {
        [SerializeField]
        private Button _button;

        public Button Button
        {
            get
            {
                Assert.IsNotNull(_button, $"[OptionButton::Button] Option: {name} is not set button.");
                return _button;
            }
        }
    }
}