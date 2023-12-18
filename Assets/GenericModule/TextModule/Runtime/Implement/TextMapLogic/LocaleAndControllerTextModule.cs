namespace CizaTextModule.Implement
{
    public class LocaleAndControllerTextModule
    {
        private readonly TextModuleWithTextMap _localeTextModule;
        private readonly TextModuleWithTextMap _controllerTextModule;

        public LocaleAndControllerTextModule(ITextModuleConfig localeTextModuleConfig, ITextModuleConfig controllerTextModuleConfig)
        {
            _localeTextModule = new TextModuleWithTextMap(localeTextModuleConfig, "LocaleTextModule", StringExtension.LocaleTextKeyPattern);
            _controllerTextModule = new TextModuleWithTextMap(controllerTextModuleConfig, "ControllerTextModule", StringExtension.ControllerTextKeyPattern);
        }

        public bool TryChangeLocaleCategory(string category) =>
            _localeTextModule.TryChangeCategory(category);

        public bool TryChangeControllerCategory(string category) =>
            _controllerTextModule.TryChangeCategory(category);

        public bool TryGetLocaleText(string localeTextKey, out string localeText) =>
            _localeTextModule.TryGetText(localeTextKey, out localeText);

        public bool TryGetControllerText(string controllerTextKey, out string controllerText) =>
            _controllerTextModule.TryGetText(controllerTextKey, out controllerText);

        public void AddTextMap(ITextMap textMap)
        {
            _localeTextModule.AddTextMap(textMap);
            _controllerTextModule.AddTextMap(textMap);
        }

        public void RemoveTextMap(ITextMap textMap)
        {
            _localeTextModule.RemoveTextMap(textMap);
            _controllerTextModule.RemoveTextMap(textMap);
        }

        public void AddTextMaps(ITextMap[] textMaps)
        {
            _localeTextModule.AddTextMaps(textMaps);
            _controllerTextModule.AddTextMaps(textMaps);
        }

        public void RemoveTextMaps(ITextMap[] textMaps)
        {
            _localeTextModule.RemoveTextMaps(textMaps);
            _controllerTextModule.RemoveTextMaps(textMaps);
        }
    }
}