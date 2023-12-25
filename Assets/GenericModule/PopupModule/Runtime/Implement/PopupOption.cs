using UnityEngine;
using UnityEngine.UI;

namespace CizaPopupModule.Implement
{
    public class PopupOption : MonoBehaviour
    {
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


        private string _animName;

        public Button Button => _button;


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
    }
}