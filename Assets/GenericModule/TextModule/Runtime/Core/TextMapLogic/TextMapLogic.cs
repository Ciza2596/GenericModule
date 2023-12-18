using System;
using System.Collections.Generic;
using System.Linq;

namespace CizaTextModule
{
    public class TextMapLogic
    {
        public interface ITextModule
        {
            public static ITextModule CreateTextModule(string dataId, TextModule textModule, string keyPattern) =>
                new TextModuleImp(dataId, textModule, keyPattern);

            event Action<string> OnChangeCategory;

            string DataId { get; }
            string KeyPattern { get; }

            string[] Categories { get; }

            string DefaultCategory { get; }
            string CurrentCategory { get; }

            bool TryChangeCategory(string category);

            bool TryGetText(string key, out string text);
            bool TryGetTexts(string[] keys, out Dictionary<string, string> textMapByKey);


            private class TextModuleImp : ITextModule
            {
                private readonly TextModule _textModule;

                public TextModuleImp(string dataId, TextModule textModule, string keyPattern)
                {
                    DataId = dataId;
                    _textModule = textModule;
                    KeyPattern = keyPattern;

                    _textModule.OnChangeCategory += OnChangeCategoryImp;
                }

                public event Action<string> OnChangeCategory;

                public string DataId { get; }
                public string KeyPattern { get; }

                public string[] Categories => _textModule.Categories;

                public string DefaultCategory => _textModule.DefaultCategory;
                public string CurrentCategory => _textModule.CurrentCategory;

                public bool TryChangeCategory(string category) =>
                    _textModule.TryChangeCategory(category);

                public bool TryGetText(string key, out string text) =>
                    _textModule.TryGetText(key, out text);

                public bool TryGetTexts(string[] keys, out Dictionary<string, string> textMapByKey) =>
                    _textModule.TryGetTexts(keys, out textMapByKey);

                private void OnChangeCategoryImp(string currentCategory) =>
                    OnChangeCategory?.Invoke(currentCategory);
            }
        }

        private readonly Dictionary<string, ITextModule> _textModuleMapByDataId;
        private readonly string _className;

        private readonly HashSet<ITextMap> _textMaps = new HashSet<ITextMap>();

        public bool TryGetCategories(string dataId, out string[] categories)
        {
            if (!_textModuleMapByDataId.TryGetValue(dataId, out var textModule))
            {
                categories = Array.Empty<string>();
                return false;
            }

            categories = textModule.Categories;
            return true;
        }

        public bool TryGetDefaultCategory(string dataId, out string defaultCategory)
        {
            if (!_textModuleMapByDataId.TryGetValue(dataId, out var textModule))
            {
                defaultCategory = string.Empty;
                return false;
            }

            defaultCategory = textModule.DefaultCategory;
            return true;
        }

        public bool TryGetCurrentCategory(string dataId, out string currentCategory)
        {
            if (!_textModuleMapByDataId.TryGetValue(dataId, out var textModule))
            {
                currentCategory = string.Empty;
                return false;
            }

            currentCategory = textModule.CurrentCategory;
            return true;
        }
        
        public TextMapLogic(ITextModule[] textModules, string className)
        {
            _textModuleMapByDataId = new Dictionary<string, ITextModule>();

            foreach (var textModule in textModules)
                _textModuleMapByDataId.Add(textModule.DataId, textModule);

            _className = className;

            foreach (var textModule in textModules)
                textModule.OnChangeCategory += OnChangeCategoryImp;
        }


        public bool TryChangeCategory(string dataId, string category)
        {
            if (!_textModuleMapByDataId.TryGetValue(dataId, out var textModule))
                return false;

            if (textModule.TryChangeCategory(category))
            {
                RefreshAllTextMap();
                return true;
            }

            return false;
        }

        public bool TryGetText(string dataId, string key, out string text)
        {
            if (!_textModuleMapByDataId.TryGetValue(dataId, out var textModule))
            {
                text = string.Empty;
                return false;
            }

            return textModule.TryGetText(key, out text);
        }


        public void AddTextMap(ITextMap textMap)
        {
            SetTextMap(textMap);
            _textMaps.Add(textMap);
        }

        public void AddTextMaps(ITextMap[] textMaps)
        {
            foreach (var textMap in textMaps)
                AddTextMap(textMap);
        }

        public void RemoveTextMap(ITextMap textMap) =>
            _textMaps.Remove(textMap);

        public void RemoveTextMaps(ITextMap[] textMaps)
        {
            foreach (var textMap in textMaps)
                RemoveTextMap(textMap);
        }

        public void RefreshAllTextMap()
        {
            foreach (var textMap in _textMaps.ToArray())
                SetTextMap(textMap);
        }

        private void OnChangeCategoryImp(string currentCategory)
        {
            foreach (var textMap in _textMaps)
                SetTextMap(textMap);
        }

        private void SetTextMap(ITextMap textMap)
        {
            if (!textMap.IsEnable)
                return;

            if (TrySetTextMapByKey(textMap))
                return;

            if (TrySetTextMapByKeyWithPattern(textMap))
                return;
        }


        private bool TrySetTextMapByKey(ITextMap textMap)
        {
            foreach (var textModule in _textModuleMapByDataId.Values.ToArray())
                if (textModule.TryGetText(textMap.Key, out var text))
                {
                    textMap.SetText(text);
                    return true;
                }

            return false;
        }

        private bool TrySetTextMapByKeyWithPattern(ITextMap textMap)
        {
            var text = textMap.Key;

            foreach (var textModule in _textModuleMapByDataId.Values.ToArray())
            {
                var keys = text.GetKeys(textModule.KeyPattern);
                if (textModule.TryGetTexts(keys, out var textMapByKey))
                    text = text.Replace(textModule.KeyPattern, textMapByKey, _className, "TrySetTextMapByKeyWithPattern");
            }

            if (!string.IsNullOrEmpty(text) && text != textMap.Key)
            {
                textMap.SetText(text);
                return true;
            }

            return false;
        }
    }
}