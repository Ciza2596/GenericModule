namespace CizaInputModule
{
	public interface IRumbleManagerConfig
	{
		bool TryGetRumbleInfo(string dataId, out IRumbleInfo rumbleInfo);
	}
}
