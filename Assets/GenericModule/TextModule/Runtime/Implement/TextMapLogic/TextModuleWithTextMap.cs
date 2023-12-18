using System.Collections.Generic;

namespace CizaTextModule.Implement
{
    public class TextModuleWithTextMap
    {
        private readonly TextMapLogic _textMapLogic;

        public bool TryGetCategories(string dataId, out string[] categories) =>
            _textMapLogic.TryGetCategories(dataId, out categories);

        public bool TryGetDefaultCategory(string dataId, out string defaultCategory) =>
            _textMapLogic.TryGetDefaultCategory(dataId, out defaultCategory);

        public bool TryGetCurrentCategory(string dataId, out string currentCategory) =>
            _textMapLogic.TryGetCurrentCategory(dataId, out currentCategory);

        public TextModuleWithTextMap(Map[] maps, string className)
        {
            var textModules = new List<TextMapLogic.ITextModule>();
            foreach (var map in maps)
                textModules.Add(TextMapLogic.ITextModule.CreateTextModule(map.DataId, new TextModule(map.TextModuleConfig), map.KeyPattern));

            _textMapLogic = new TextMapLogic(textModules.ToArray(), className);
        }

        public bool TryChangeCategory(string dataId, string category) =>
            _textMapLogic.TryChangeCategory(dataId, category);

        public bool TryGetText(string dataId, string key, out string text) =>
            _textMapLogic.TryGetText(dataId, key, out text);

        public void AddTextMap(ITextMap textMap) =>
            _textMapLogic.AddTextMap(textMap);

        public void RemoveTextMap(ITextMap textMap) =>
            _textMapLogic.RemoveTextMap(textMap);

        public void AddTextMaps(ITextMap[] textMaps) =>
            _textMapLogic.AddTextMaps(textMaps);

        public void RemoveTextMaps(ITextMap[] textMaps) =>
            _textMapLogic.RemoveTextMaps(textMaps);

        public class Map
        {
            public string DataId { get; }

            public ITextModuleConfig TextModuleConfig { get; }

            public string KeyPattern { get; }

            public Map(string dataId, ITextModuleConfig textModuleConfig, string keyPattern)
            {
                DataId = dataId;
                TextModuleConfig = textModuleConfig;
                KeyPattern = keyPattern;
            }
        }
    }
}