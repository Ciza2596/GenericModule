namespace CizaTextModule.Implement
{
    public class LocaleAndControllerTextModule
    {
        public const string LocaleTextModuleDataId = "LocaleTextModule";
        public const string ControllerTextModuleDataId = "ControllerTextModule";

        private readonly TextModuleWithTextMap _textModuleWithTextMap;

        public LocaleAndControllerTextModule(ITextModuleConfig localeTextModuleConfig, ITextModuleConfig controllerTextModuleConfig)
        {
            var maps = new[] { new TextModuleWithTextMap.Map(LocaleTextModuleDataId, localeTextModuleConfig, StringExtension.LocaleTextKeyPattern), new TextModuleWithTextMap.Map(ControllerTextModuleDataId, controllerTextModuleConfig, StringExtension.ControllerTextKeyPattern) };

            _textModuleWithTextMap = new TextModuleWithTextMap(maps, "LocaleAndControllerTextModule");
        }

        public bool TryChangeLocaleCategory(string category) =>
            _textModuleWithTextMap.TryChangeCategory(LocaleTextModuleDataId, category);

        public bool TryChangeControllerCategory(string category) =>
            _textModuleWithTextMap.TryChangeCategory(ControllerTextModuleDataId, category);

        public bool TryGetLocaleText(string localeTextKey, out string localeText) =>
            _textModuleWithTextMap.TryGetText(LocaleTextModuleDataId, localeTextKey, out localeText);

        public bool TryGetControllerText(string controllerTextKey, out string controllerText) =>
            _textModuleWithTextMap.TryGetText(ControllerTextModuleDataId, controllerTextKey, out controllerText);

        public void AddTextMap(ITextMap textMap) =>
            _textModuleWithTextMap.AddTextMap(textMap);

        public void RemoveTextMap(ITextMap textMap) =>
            _textModuleWithTextMap.RemoveTextMap(textMap);

        public void AddTextMaps(ITextMap[] textMaps) =>
            _textModuleWithTextMap.AddTextMaps(textMaps);

        public void RemoveTextMaps(ITextMap[] textMaps) =>
            _textModuleWithTextMap.RemoveTextMaps(textMaps);
    }
}