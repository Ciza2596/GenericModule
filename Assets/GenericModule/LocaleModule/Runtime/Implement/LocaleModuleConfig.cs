using UnityEngine;

namespace CizaLocaleModule
{
    [CreateAssetMenu(fileName = "LocaleModuleConfig", menuName = "Ciza/LocaleModule/LocaleModuleConfig")]
    public class LocaleModuleConfig : ScriptableObject, ILocaleModuleConfig
    {
        [SerializeField]
        private string[] _supportLocales = new[] { "en" };

        [Space]
        [SerializeField]
        private bool _isIgnoreSourceLocale = true;

        [SerializeField]
        private string _sourceLocale = "en";

        [Space]
        [SerializeField]
        private string _defaultLocale = "en";

        [Space]
        [SerializeField]
        private char _prefixTag = '/';

        public string[] SupportLocales => _supportLocales;

        public bool IsIgnoreSourceLocale => _isIgnoreSourceLocale;
        public string SourceLocale => _sourceLocale;

        public string DefaultLocale => _defaultLocale;
        public char PrefixTag => _prefixTag;
    }
}