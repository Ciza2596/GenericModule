namespace CizaPreloadController
{
	public interface IPreloadAssetInfo
	{
		bool IsPreLoad { get; }

		string Address { get; }

		AssetTypes AssetType { get; }
	}
}
