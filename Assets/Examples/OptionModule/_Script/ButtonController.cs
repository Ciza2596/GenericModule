using System;
using UnityEngine;
using UnityEngine.UI;

namespace CizaInputModule.Example
{
    public class ButtonController : MonoBehaviour
    {
        [SerializeField]
        private Button _button;

        public event Action OnClick;
        
        private void Awake()
        {
            _button.onClick.AddListener(OnClickImp);
        }
        
        private void OnClickImp()
        {
            OnClick?.Invoke();
        }
    }
}