namespace CizaTextModule.Implement
{
    public class TextModuleWithTextMap
    {
        private readonly TextModule _textModule;
        private readonly TextMapLogic _textMapLogic;

        public TextModuleWithTextMap(ITextModuleConfig textModuleConfig, string className, string keyPattern)
        {
            _textModule = new TextModule(textModuleConfig);
            _textMapLogic = new TextMapLogic(TextMapLogic.ITextModule.CreateTextModule(_textModule), className, keyPattern);
        }

        public bool TryChangeCategory(string category) =>
            _textModule.TryChangeCategory(category);

        public bool TryGetText(string key, out string text) =>
            _textModule.TryGetText(key, out text);

        public void AddTextMap(ITextMap textMap) =>
            _textMapLogic.AddTextMap(textMap);

        public void RemoveTextMap(ITextMap textMap) =>
            _textMapLogic.RemoveTextMap(textMap);

        public void AddTextMaps(ITextMap[] textMaps) =>
            _textMapLogic.AddTextMaps(textMaps);

        public void RemoveTextMaps(ITextMap[] textMaps) =>
            _textMapLogic.RemoveTextMaps(textMaps);
    }
}