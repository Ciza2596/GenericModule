using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace CizaOptionModule.Implement
{
	public class OptionConfirmButton : OptionSubMono
	{
		[SerializeField]
		private Button _button;

		public override void Initialize(Option option)
		{
			base.Initialize(option);
			_button.onClick.AddListener(OnClick);

			Assert.IsNotNull(_button, $"[{GetType().Name}::Awake] Button is not referenced.");
		}

		protected void OnClick() =>
			_option.TryConfirm();
	}
}
