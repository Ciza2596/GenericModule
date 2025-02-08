namespace CizaAddressablesModule.Preload
{
	public interface IPreloadAssetInfo
	{
		bool IsPreLoad { get; }

		string Address { get; }

		AssetKinds AssetKind { get; }
	}
}
