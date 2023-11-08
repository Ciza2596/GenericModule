namespace CizaOptionModule.Implement
{
	public class OptionIsNewMark : OptionSubMono
	{
		public override void Initialize(Option option)
		{
			base.Initialize(option);

			_option.OnIsNew += OnIsNew;
		}

		private void OnIsNew(string optionKey, bool isNew) =>
			gameObject.SetActive(isNew);
	}
}
