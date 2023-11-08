using UnityEngine;
using UnityEngine.Assertions;

namespace CizaOptionModule.Implement
{
	public class OptionAnimator : OptionSubMono
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

			_option.OnSelect   += key => Play(_select);
			_option.OnUnselect += key => Play(_unselect);
			_option.OnConfirm  += (key, isUnlock) => Play(isUnlock ? _confirm : _cantConfirm);

			Assert.IsNotNull(_animator, $"[{GetType().Name}::Awake] Animator is not referenced.");
		}

		public void OnEnable() =>
			Play();

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
