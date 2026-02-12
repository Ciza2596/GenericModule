using UnityEngine;
using UnityEngine.Assertions;

namespace CizaOptionModule.Implement
{
    public class OptionAnimator : OptionSup
    {
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

        [SerializeField]
        private string _cantConfirm = "CantConfirm";

        private string _animName;

        public override void Initialize(Option option)
        {
            base.Initialize(option);

            Option.OnSelect += OnSelect;
            Option.OnUnselect += OnUnselect;
            Option.OnConfirm += OnConfirm;

            Assert.IsNotNull(_animator, $"[{GetType().Name}::Awake] Animator is not referenced.");
        }

        public override void Release(Option option)
        {
            base.Release(option);

            Option.OnSelect -= OnSelect;
            Option.OnUnselect -= OnUnselect;
            Option.OnConfirm -= OnConfirm;
        }

        public void OnEnable() =>
            Play();


        private void OnSelect(string key) =>
            Play(_select);

        private void OnUnselect(string key) =>
            Play(_unselect);

        private void OnConfirm(string key, bool isUnlock) =>
            Play(isUnlock ? _confirm : _cantConfirm);

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