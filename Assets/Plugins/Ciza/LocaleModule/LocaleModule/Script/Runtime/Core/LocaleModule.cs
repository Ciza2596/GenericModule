using System;
using System.Linq;
using CizaUniTask;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaLocaleModule
{
    public class LocaleModule
    {
        private readonly string _className;
        private readonly ILocaleModuleConfig _config;
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
        public LocaleModule(ILocaleModuleConfig config) : this("LocaleModule", config) { }

        [Preserve]
        public LocaleModule(string className, ILocaleModuleConfig config)
        {
            _className = className;
            _config = config;

            if (!_config.SupportLocales.Contains(_config.SourceLocale))
                Debug.LogError($"[{_className}::LocaleModule] SourceLocale: {_config.SourceLocale} is not in supportLocales.");

            if (!_config.SupportLocales.Contains(_config.DefaultLocale))
                Debug.LogError($"[{_className}::LocaleModule] DefaultLocale: {_config.DefaultLocale} is not in supportLocales.");
        }

        public void Initialize()
        {
            if (IsInitialized)
            {
                Debug.LogWarning($"[{_className}::Initialize] LocaleModule is initialized.");
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
                Debug.LogWarning($"[{_className}::Release] LocaleModule is not initialized.");
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
                Debug.LogWarning($"[{_className}::ChangeLocale] LocaleModule is not initialized.");
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
                Debug.LogWarning($"[{_className}::GetTextWithLocalePrefix] LocaleModule is not initialized.");
                return string.Empty;
            }

            var localePrefix = _config.IsIgnoreSourceLocale && CurrentLocale.Equals(SourceLocale) ? string.Empty : CurrentLocale + _config.PrefixTag;
            var textByAddLocalePrefix = localePrefix + text;

            return textByAddLocalePrefix;
        }
    }
}