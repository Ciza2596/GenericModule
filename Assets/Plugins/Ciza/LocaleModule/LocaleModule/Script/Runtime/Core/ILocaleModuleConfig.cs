namespace CizaLocaleModule
{
    public interface ILocaleModuleConfig
    {
        string[] SupportLocales { get; }
        
        string SourceLocale { get; }
        bool IsIgnoreSourceLocalePrefix { get; }

        string DefaultLocale { get; }

        char PrefixTag { get; }
    }
}