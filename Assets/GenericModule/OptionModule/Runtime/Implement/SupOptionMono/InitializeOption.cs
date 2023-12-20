namespace CizaOptionModule.Implement
{
	public abstract class InitializeOption : OptionSup
	{
		public override void Initialize(Option option)
		{
			base.Initialize(option);

			Option.OnInitialize += OnInitialize;
		}

		public override void Release(Option option)
		{
			base.Release(option);
			Option.OnInitialize -= OnInitialize;
		}

		protected abstract void OnInitialize(object[] parameters);
	}
}
