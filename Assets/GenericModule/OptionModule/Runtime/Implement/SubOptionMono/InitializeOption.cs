namespace CizaOptionModule.Implement
{
	public abstract class InitializeOption : OptionSubMono
	{
		public override void Initialize(Option option)
		{
			base.Initialize(option);

			_option.OnInitialize += OnInitialize;
		}

		protected abstract void OnInitialize(object[] parameters);
	}
}
