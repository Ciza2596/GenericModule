namespace CizaLocalizationModule
{
    public interface ILocalizationModuleConfig
    {
        string[] SupportLocales { get; }

        bool IsIgnoreSourceLocale { get; }
        string SourceLocale { get; }

        string DefaultLocale { get; }

        char PrefixTag { get; }
    }
}