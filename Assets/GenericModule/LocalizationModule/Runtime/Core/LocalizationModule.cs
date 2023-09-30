using System;
using System.Linq;
using UnityEngine;

namespace CizaLocalizationModule
{
	public class LocalizationModule
	{
		private readonly ILocalizationModuleConfig _config;
		private          string[]                  _supportLocales;

		public event Action<string> OnChangeLocale;

		public bool IsInitialized { get; private set; }

		public string   DefaultLocale  { get; private set; }
		public string[] SupportLocales => _supportLocales != null ? _supportLocales.ToArray() : Array.Empty<string>();
		public string   CurrentLocale  { get; private set; }
		public string   SourceLocale   { get; private set; }

		public LocalizationModule(ILocalizationModuleConfig config)
		{
			_config = config;

			if (!_config.SupportLocales.Contains(_config.SourceLocale))
				Debug.LogError($"[LocalizationModule::LocalizationModule] SourceLocale: {_config.SourceLocale} is not in supportLocales.");

			if (!_config.SupportLocales.Contains(_config.DefaultLocale))
				Debug.LogError($"[LocalizationModule::LocalizationModule] DefaultLocale: {_config.DefaultLocale} is not in supportLocales.");
		}

		public void Initialize()
		{
			if (IsInitialized)
			{
				Debug.LogWarning($"[LocalizationModule::Release] LocalizationModule is initialized.");
				return;
			}

			IsInitialized = true;

			_supportLocales = _config.SupportLocales;
			SourceLocale    = _config.SourceLocale;
			DefaultLocale   = _config.DefaultLocale;
			CurrentLocale   = DefaultLocale;
		}

		public void Release()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocalizationModule::Release] LocalizationModule is not initialized.");
				return;
			}

			_supportLocales = null;
			SourceLocale    = string.Empty;
			CurrentLocale   = string.Empty;
			DefaultLocale   = string.Empty;

			IsInitialized = false;
		}

		public void ChangeToDefaultLocale()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocalizationModule::ChangeToDefaultLocale] LocalizationModule is not initialized.");
				return;
			}

			CurrentLocale = DefaultLocale;
		}

		public void ChangeLocale(string locale)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocalizationModule::ChangeLocale] LocalizationModule is not initialized.");
				return;
			}

			if (!SupportLocales.Contains(locale))
			{
				Debug.LogWarning($"[LocalizationModule::ChangeLocale] LocalizationModule is not initialized.");
				return;
			}

			CurrentLocale = locale;

			OnChangeLocale?.Invoke(CurrentLocale);
		}

		public string GetTextWithLocalePrefix(string text)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocalizationModule::GetTextWithLocalePrefix] LocalizationModule is not initialized.");
				return string.Empty;
			}

			var localePrefix          = CurrentLocale.Equals(SourceLocale) ? string.Empty : CurrentLocale + _config.PrefixMark;
			var textByAddLocalePrefix = localePrefix + text;

			return textByAddLocalePrefix;
		}

		public string GetLocaleText(string key)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocalizationModule::GetLocaleText] LocalizationModule is not initialized.");
				return string.Empty;
			}

			return string.Empty;
		}
	}
}
