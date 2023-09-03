using System;
using System.Linq;
using UnityEngine;

namespace CizaLocalizationModule
{
	public class LocalizationModule
	{
		private readonly ILocalizationModuleConfig _config;
		private          string                    _defaultLocale;
		private          Action<string>            _onChangeLocale;

		public bool     IsInitialized  { get; private set; }
		public string[] SupportLocales { get; private set; }
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

			SupportLocales = _config.SupportLocales;
			SourceLocale   = _config.SourceLocale;
			_defaultLocale = _config.DefaultLocale;
			CurrentLocale  = _defaultLocale;
		}

		public void Release()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocalizationModule::Release] LocalizationModule is not initialized.");
				return;
			}

			SupportLocales = null;
			SourceLocale   = string.Empty;
			CurrentLocale  = string.Empty;
			_defaultLocale = string.Empty;

			RemoveAllOnChangeLocale();
			IsInitialized = false;
		}

		public void AddOnChangeLocale(Action<string> onChangeLocale)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocalizationModule::AddOnChangeLocale] LocalizationModule is not initialized.");
				return;
			}

			_onChangeLocale += onChangeLocale;
		}

		public void RemoveOnChangeLocale(Action<string> onChangeLocale)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocalizationModule::RemoveOnChangeLocale] LocalizationModule is not initialized.");
				return;
			}

			_onChangeLocale -= onChangeLocale;
		}

		public void RemoveAllOnChangeLocale()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocalizationModule::RemoveAllOnChangeLocale] LocalizationModule is not initialized.");
				return;
			}

			_onChangeLocale = null;
		}

		public void ChangeToDefaultLocale()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocalizationModule::ChangeToDefaultLocale] LocalizationModule is not initialized.");
				return;
			}

			CurrentLocale = _defaultLocale;
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

			_onChangeLocale?.Invoke(CurrentLocale);
		}

		public string GetTextByAddLocalePrefix(string text)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocalizationModule::GetTextByAddLocalePrefix] LocalizationModule is not initialized.");
				return string.Empty;
			}

			var localePrefix          = CurrentLocale.Equals(SourceLocale) ? string.Empty : CurrentLocale + _config.PrefixMark;
			var textByAddLocalePrefix = localePrefix + text;

			return textByAddLocalePrefix;
		}
	}
}
