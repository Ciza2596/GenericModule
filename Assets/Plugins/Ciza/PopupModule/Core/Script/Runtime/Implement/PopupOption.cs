using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CizaPopupModule.Implement
{
    public class PopupOption : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField]
        private TMP_Text _text;

        [SerializeField]
        private Button _button;

        [Space]
        [SerializeField]
        private Animator _animator;

        [Space]
        [SerializeField]
        private string _select = "Select";

        [SerializeField]
        private string _unselect = "Unselect";

        [Space]
        [SerializeField]
        private string _confirm = "Confirm";


        private Action<string, int> _onSelect;
        private string _animName;

        public Button Button => _button;

        public string Key { get; private set; }
        public int Index { get; private set; }

        public void Initialize(string key, int index, Action<string, int> onSelect)
        {
            Key = key;
            Index = index;
            _onSelect = onSelect;
        }

        public void SetText(string text) =>
            _text.text = text;

        public void Select() =>
            Play(_select);

        public void Unselect() =>
            Play(_unselect);

        public void Confirm() =>
            Play(_confirm);


        private void Play(string animName)
        {
            _animName = animName;
            Play();
        }

        private void Play()
        {
            if (!_animator.isActiveAndEnabled)
                return;

            _animator.Play(_animName, 0, 0);
        }

        public void OnPointerEnter(PointerEventData eventData) =>
            _onSelect?.Invoke(Key, Index);
    }
}