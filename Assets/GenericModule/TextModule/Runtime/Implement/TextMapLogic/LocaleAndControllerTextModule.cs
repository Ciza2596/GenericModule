using System;

namespace CizaTextModule.Implement
{
    public class LocaleAndControllerTextModule
    {
        public const string LocaleTextModuleDataId = "LocaleTextModule";
        public const string ControllerTextModuleDataId = "ControllerTextModule";

        private readonly TextModuleWithTextMap _textModuleWithTextMap;

        public event Action<string> OnChangeLocaleCategory;
        public event Action<string> OnChangeControllerCategory;

        public string[] LocaleCategories
        {
            get
            {
                _textModuleWithTextMap.TryGetCategories(LocaleTextModuleDataId, out var categories);
                return categories;
            }
        }

        public string LocaleDefaultCategory
        {
            get
            {
                _textModuleWithTextMap.TryGetDefaultCategory(LocaleTextModuleDataId, out var defaultCategory);
                return defaultCategory;
            }
        }

        public string LocaleCurrentCategory
        {
            get
            {
                _textModuleWithTextMap.TryGetCurrentCategory(LocaleTextModuleDataId, out var currentCategory);
                return currentCategory;
            }
        }


        public string[] ControllerCategories
        {
            get
            {
                _textModuleWithTextMap.TryGetCategories(ControllerTextModuleDataId, out var categories);
                return categories;
            }
        }

        public string ControllerDefaultCategory
        {
            get
            {
                _textModuleWithTextMap.TryGetDefaultCategory(ControllerTextModuleDataId, out var defaultCategory);
                return defaultCategory;
            }
        }

        public string ControllerCurrentCategory
        {
            get
            {
                _textModuleWithTextMap.TryGetCurrentCategory(ControllerTextModuleDataId, out var currentCategory);
                return currentCategory;
            }
        }

        public bool TryGetLocaleText(string localeTextKey, out string localeText) =>
            _textModuleWithTextMap.TryGetText(LocaleTextModuleDataId, localeTextKey, out localeText);

        public bool TryGetControllerText(string controllerTextKey, out string controllerText) =>
            _textModuleWithTextMap.TryGetText(ControllerTextModuleDataId, controllerTextKey, out controllerText);

        public LocaleAndControllerTextModule(ITextModuleConfig localeTextModuleConfig, ITextModuleConfig controllerTextModuleConfig)
        {
            var maps = new[] { new TextModuleWithTextMap.Map(LocaleTextModuleDataId, localeTextModuleConfig, StringExtension.LocaleTextKeyPattern), new TextModuleWithTextMap.Map(ControllerTextModuleDataId, controllerTextModuleConfig, StringExtension.ControllerTextKeyPattern) };
            _textModuleWithTextMap = new TextModuleWithTextMap(maps, "LocaleAndControllerTextModule");

            _textModuleWithTextMap.OnChangeCategory += OnChangeCategoryImp;
        }

        public bool TryChangeLocaleCategory(string category) =>
            _textModuleWithTextMap.TryChangeCategory(LocaleTextModuleDataId, category);

        public bool TryChangeControllerCategory(string category) =>
            _textModuleWithTextMap.TryChangeCategory(ControllerTextModuleDataId, category);

        public void RefreshAllTextMaps() =>
            _textModuleWithTextMap.RefreshAllTextMaps();

        public void AddTextMap(ITextMap textMap) =>
            _textModuleWithTextMap.AddTextMap(textMap);

        public void RemoveTextMap(ITextMap textMap) =>
            _textModuleWithTextMap.RemoveTextMap(textMap);

        public void AddTextMaps(ITextMap[] textMaps) =>
            _textModuleWithTextMap.AddTextMaps(textMaps);

        public void RemoveTextMaps(ITextMap[] textMaps) =>
            _textModuleWithTextMap.RemoveTextMaps(textMaps);

        private void OnChangeCategoryImp(string dataId, string category)
        {
            if (dataId == LocaleTextModuleDataId)
                OnChangeLocaleCategory?.Invoke(category);

            else if (dataId == ControllerTextModuleDataId)
                OnChangeControllerCategory?.Invoke(category);
        }
    }
}