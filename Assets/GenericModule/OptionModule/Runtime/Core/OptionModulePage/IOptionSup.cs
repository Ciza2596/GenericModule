namespace CizaOptionModule
{
	public interface IOptionSup
	{
		bool IsInitialized { get; }

		void Initialize(Option option);

		void Release(Option option);
	}
}
