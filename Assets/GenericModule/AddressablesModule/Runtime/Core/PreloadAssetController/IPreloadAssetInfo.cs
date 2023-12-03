namespace CizaAddressablesModule.PreloadAssetController
{
	public interface IPreloadAssetInfo
	{
		bool IsPreLoad { get; }

		string Address { get; }

		AssetKinds AssetKind { get; }
	}
}
