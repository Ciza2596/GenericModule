using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;
using UniTask = Cysharp.Threading.Tasks.UniTask;

namespace CizaLocalizationModule
{
    public class LocalizationModule
    {
        private readonly string _className;
        private readonly ILocalizationModuleConfig _config;
        private string[] _supportLocales;

        public event Func<string, UniTask> OnChangedLocaleBeforeAsync;
        public event Func<string, UniTask> OnChangedLocaleAsync;

        public bool IsInitialized { get; private set; }

        public bool IsChangingLocale { get; private set; }

        public string DefaultLocale { get; private set; }
        public string[] SupportLocales => _supportLocales != null ? _supportLocales.ToArray() : Array.Empty<string>();
        public string CurrentLocale { get; private set; }
        public string SourceLocale { get; private set; }

        [Preserve]
        public LocalizationModule(ILocalizationModuleConfig config) : this("LocalizationModule", config) { }

        [Preserve]
        public LocalizationModule(string className, ILocalizationModuleConfig config)
        {
            _className = className;
            _config = config;

            if (!_config.SupportLocales.Contains(_config.SourceLocale))
                Debug.LogError($"[{_className}::LocalizationModule] SourceLocale: {_config.SourceLocale} is not in supportLocales.");

            if (!_config.SupportLocales.Contains(_config.DefaultLocale))
                Debug.LogError($"[{_className}::LocalizationModule] DefaultLocale: {_config.DefaultLocale} is not in supportLocales.");
        }

        public void Initialize()
        {
            if (IsInitialized)
            {
                Debug.LogWarning($"[{_className}::Initialize] LocalizationModule is initialized.");
                return;
            }

            IsInitialized = true;

            _supportLocales = _config.SupportLocales;
            SourceLocale = _config.SourceLocale;
            DefaultLocale = _config.DefaultLocale;
            CurrentLocale = DefaultLocale;
        }

        public void Release()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning($"[{_className}::Release] LocalizationModule is not initialized.");
                return;
            }

            _supportLocales = null;
            SourceLocale = string.Empty;
            CurrentLocale = string.Empty;
            DefaultLocale = string.Empty;

            IsInitialized = false;
        }

        public UniTask ChangeToDefaultLocaleAsync() =>
            ChangeLocaleAsync(DefaultLocale);


        public async UniTask ChangeLocaleAsync(string locale)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning($"[{_className}::ChangeLocale] LocalizationModule is not initialized.");
                return;
            }

            if (!SupportLocales.Contains(locale))
            {
                Debug.LogError($"[{_className}::ChangeLocale] Locale: {locale} is not support.");
                return;
            }

            if (IsChangingLocale)
                return;

            IsChangingLocale = true;

            await m_OnChangedLocaleBeforeAsync();

            CurrentLocale = locale;

            await m_OnChangedLocaleAsync();

            IsChangingLocale = false;


            UniTask m_OnChangedLocaleBeforeAsync() =>
                OnChangedLocaleBeforeAsync?.Invoke(CurrentLocale) ?? UniTask.CompletedTask;

            UniTask m_OnChangedLocaleAsync() =>
                OnChangedLocaleAsync?.Invoke(CurrentLocale) ?? UniTask.CompletedTask;
        }

        public string GetTextWithLocalePrefix(string text)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning($"[{_className}::GetTextWithLocalePrefix] LocalizationModule is not initialized.");
                return string.Empty;
            }

            var localePrefix = _config.IsIgnoreSourceLocale && CurrentLocale.Equals(SourceLocale) ? string.Empty : CurrentLocale + _config.PrefixTag;
            var textByAddLocalePrefix = localePrefix + text;

            return textByAddLocalePrefix;
        }
    }
}