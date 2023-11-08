namespace CizaOptionModule
{
	public interface IOptionInfo
	{
		string Key { get; }

		bool IsEnable { get; }
		bool IsUnlock { get; }
		bool IsNew    { get; }

		object[] Parameters { get; }
	}
}
