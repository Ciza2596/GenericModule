namespace CizaLocalizationModule
{
	public interface ILocalizationModuleConfig
	{
		
		string[] SupportLocales { get; }

		string SourceLocale { get; }

		string DefaultLocale { get; }

		char PrefixMark { get; }
	}
}
