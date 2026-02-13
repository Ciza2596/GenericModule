namespace CizaTextModule
{
	public interface ITextModuleConfig
	{
		bool TryGetCsvText(out string csvText);
	}
}