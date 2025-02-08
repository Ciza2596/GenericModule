namespace CizaInputModule
{
	public interface IRumbleManagerConfig
	{
		string[] AllDataIds { get; }

		bool TryGetRumbleInfo(string dataId, out IRumbleInfo rumbleInfo);
	}
}
