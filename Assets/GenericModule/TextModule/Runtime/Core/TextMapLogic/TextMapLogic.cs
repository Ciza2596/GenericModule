using System;
using System.Collections.Generic;

namespace CizaTextModule
{
    public class TextMapLogic
    {
        public interface ITextModule
        {
            public static ITextModule CreateTextModule(TextModule textModule) =>
                new TextModuleImp(textModule);

            event Action<string> OnChangeCategory;

            bool TryGetText(string key, out string text);
            bool TryGetTexts(string[] keys, out Dictionary<string, string> textMapByKey);


            private class TextModuleImp : ITextModule
            {
                private readonly TextModule _textModule;

                public TextModuleImp(TextModule textModule)
                {
                    _textModule = textModule;
                    _textModule.OnChangeCategory += OnChangeCategoryImp;
                }

                public event Action<string> OnChangeCategory;

                public bool TryGetText(string key, out string text) =>
                    _textModule.TryGetText(key, out text);

                public bool TryGetTexts(string[] keys, out Dictionary<string, string> textMapByKey) =>
                    _textModule.TryGetTexts(keys, out textMapByKey);

                private void OnChangeCategoryImp(string currentCategory) =>
                    OnChangeCategory?.Invoke(currentCategory);
            }
        }

        private readonly ITextModule _textModule;
        private readonly string _className;

        private readonly HashSet<ITextMap> _textMaps = new HashSet<ITextMap>();

        public string KeyPattern { get; }

        public TextMapLogic(ITextModule textModule, string className, string keyPattern)
        {
            _textModule = textModule;
            _className = className;
            KeyPattern = keyPattern;
            _textModule.OnChangeCategory += OnChangeCategoryImp;
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
            if (_textModule.TryGetText(textMap.Key, out var text))
            {
                textMap.SetText(text);
                return true;
            }

            return false;
        }

        private bool TrySetTextMapByKeyWithPattern(ITextMap textMap)
        {
            var text = textMap.Key;
            var keys = textMap.Key.GetKeys(KeyPattern);
            if (_textModule.TryGetTexts(keys, out var textMapByKey))
                text = text.Replace(KeyPattern, textMapByKey, _className, "TrySetTextMapByKeyWithPattern");

            if (!string.IsNullOrEmpty(text) && text != textMap.Key)
            {
                textMap.SetText(text);
                return true;
            }

            return false;
        }
    }
}