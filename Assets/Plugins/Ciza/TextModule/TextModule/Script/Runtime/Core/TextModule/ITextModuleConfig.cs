namespace CizaTextModule
{
	public interface ITextModuleConfig
	{
		bool TryGetCsv(out string csv);
	}
}